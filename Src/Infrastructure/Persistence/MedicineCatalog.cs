using Application.DTOs.Medicines;
using Application.Interfaces;
using Application.Wrappers;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class MedicineCatalog : IMedicineCatalog
{
    private readonly ApplicationDbContext _db;

    public MedicineCatalog(ApplicationDbContext db) => _db = db;

    public async Task<PaginationResponse<MedicineDto>> SearchAsync(RequestParametersBasic request, CancellationToken cancellationToken)
    {
        var query = ReadKeys();
        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x.Title != null && x.Title.ToLower().Contains(search)) ||
                (x.OriginalName != null && x.OriginalName.ToLower().Contains(search)) ||
                (x.ActiveIngredient != null && x.ActiveIngredient.ToLower().Contains(search)) ||
                (x.Form != null && x.Form.ToLower().Contains(search)));
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);
        var pageIndex = Math.Max(request.PageIndex, 1);
        var count = await query.CountAsync(cancellationToken);
        var offset = ((long)pageIndex - 1) * pageSize;
        if (offset >= count)
            return new PaginationResponse<MedicineDto>(pageIndex, pageSize, count, Array.Empty<MedicineDto>());

        var ordered = request.TypeSort == TypeSort.Desc
            ? query.OrderByDescending(x => x.Title)
            : query.OrderBy(x => x.Title);
        var page = ordered.ThenBy(x => x.Id).Skip((int)offset).Take(pageSize);
        var enriched = WithDetails(page);
        // A join does not guarantee the input order; reapply the same order to this page.
        var orderedPage = request.TypeSort == TypeSort.Desc
            ? enriched.OrderByDescending(x => x.Title)
            : enriched.OrderBy(x => x.Title);
        var rows = await orderedPage.ThenBy(x => x.Id).ToListAsync(cancellationToken);
        return new PaginationResponse<MedicineDto>(pageIndex, pageSize, count, rows.Select(ToDto).ToList());
    }

    public async Task<MedicineDto?> FindAsync(int id, CancellationToken cancellationToken)
    {
        var row = await WithDetails(ReadKeys().Where(x => x.Id == id)).SingleOrDefaultAsync(cancellationToken);
        return row == null ? null : ToDto(row);
    }

    private IQueryable<CatalogKey> ReadKeys() => _db.Database.SqlQueryRaw<CatalogKey>(KeySql);

    // Compose one data statement: filter/order/page first, then join the clinical text and URL.
    private IQueryable<CatalogRow> WithDetails(IQueryable<CatalogKey> keys) =>
        from k in keys
        join d in _db.Database.SqlQueryRaw<CatalogDetail>(DetailSql) on k.Id equals d.Id
        select new CatalogRow
        {
            Id = k.Id, Title = k.Title, OriginalName = k.OriginalName, Form = k.Form,
            ActiveIngredient = k.ActiveIngredient, Description = d.Description,
            SideEffects = d.SideEffects, SourceUrl = d.SourceUrl
        };

    // Installed keys: MedicineClinicalSections(MedicineId, SectionKey), MedicineCatalogReferences(MedicineId).
    // Each LEFT JOIN therefore contributes at most one row; missing sections still use the legacy fallback.
    private const string KeySql = """
        SELECT m.[Id],
            COALESCE(NULLIF(LTRIM(RTRIM(n.[TextFa])), N''), NULLIF(LTRIM(RTRIM(m.[PersianName])), N''), NULLIF(LTRIM(RTRIM(m.[Name])), N'')) AS [Title],
            NULLIF(LTRIM(RTRIM(m.[Name])), N'') AS [OriginalName],
            COALESCE(NULLIF(LTRIM(RTRIM(f.[TextFa])), N''), NULLIF(LTRIM(RTRIM(m.[DosageForm])), N'')) AS [Form],
            NULLIF(LTRIM(RTRIM(i.[TextFa])), N'') AS [ActiveIngredient]
        FROM [dbo].[Medicines] AS m
        LEFT JOIN [dbo].[MedicineClinicalSections] AS n ON n.[MedicineId] = m.[Id] AND n.[SectionKey] = 'name_fa'
        LEFT JOIN [dbo].[MedicineClinicalSections] AS f ON f.[MedicineId] = m.[Id] AND f.[SectionKey] = 'form_fa'
        LEFT JOIN [dbo].[MedicineClinicalSections] AS i ON i.[MedicineId] = m.[Id] AND i.[SectionKey] = 'ingredient_en'
        WHERE m.[IsDelete] = 0
        """;

    private const string DetailSql = """
        SELECT m.[Id],
            COALESCE(NULLIF(LTRIM(RTRIM(d.[TextFa])), N''), NULLIF(LTRIM(RTRIM(m.[Description])), N'')) AS [Description],
            COALESCE(NULLIF(LTRIM(RTRIM(a.[TextFa])), N''), NULLIF(LTRIM(RTRIM(m.[SideEffects])), N'')) AS [SideEffects],
            LTRIM(RTRIM(r.[ClinicalUrl])) AS [SourceUrl]
        FROM [dbo].[Medicines] AS m
        LEFT JOIN [dbo].[MedicineClinicalSections] AS d ON d.[MedicineId] = m.[Id] AND d.[SectionKey] = 'indication'
        LEFT JOIN [dbo].[MedicineClinicalSections] AS a ON a.[MedicineId] = m.[Id] AND a.[SectionKey] = 'adverse_effects'
        LEFT JOIN [dbo].[MedicineCatalogReferences] AS r ON r.[MedicineId] = m.[Id]
            AND LTRIM(RTRIM(r.[ClinicalUrl])) LIKE N'https://%'
        """;

    private static MedicineDto ToDto(CatalogRow row) => new()
    {
        Id = row.Id,
        Title = row.Title,
        OriginalName = row.OriginalName,
        Form = row.Form,
        Description = row.Description,
        SideEffects = row.SideEffects,
        ActiveIngredient = row.ActiveIngredient,
        SourceUrl = Uri.TryCreate(row.SourceUrl, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps
            ? row.SourceUrl
            : null,
        Dosage = null
    };

    private sealed class CatalogKey
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? OriginalName { get; set; }
        public string? Form { get; set; }
        public string? ActiveIngredient { get; set; }
    }

    private sealed class CatalogDetail
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? SideEffects { get; set; }
        public string? SourceUrl { get; set; }
    }

    private sealed class CatalogRow
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? OriginalName { get; set; }
        public string? Form { get; set; }
        public string? Description { get; set; }
        public string? SideEffects { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? SourceUrl { get; set; }
    }
}
