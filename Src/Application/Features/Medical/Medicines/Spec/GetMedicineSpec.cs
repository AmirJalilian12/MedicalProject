using Application.DTOs.Contracts.Specification;
using Application.Features.Medical.Medicines.Queries;
using Application.Wrappers;
using Domain.Entities.MedicalEntities.MedicineEntities;
using System;
using System.Linq.Expressions;

namespace Application.Features.Medical.Medicines.Spec;

public class GetMedicineSpec : BaseSpecification<Medicine>
{
    public GetMedicineSpec(GetAllMedicineQuery specParams)
        : base(MedicineExpression.ExpressionSpec(specParams))
    {
        // Sort
        if (specParams.TypeSort == TypeSort.Desc)
        {
            AddOrderByDesc(x => x.Title);
        }
        else
        {
            AddOrderBy(x => x.Title);
        }

        // Pagination
        ApplyPaging(
            specParams.PageSize * (specParams.PageIndex - 1),
            specParams.PageSize,
            true);
    }

    public GetMedicineSpec(int id) : base(x => x.Id == id)
    {
        IsPagingEnabled = false;
    }
}

public class MedicineCountSpec : BaseSpecification<Medicine>
{
    public MedicineCountSpec(GetAllMedicineQuery specParams)
        : base(MedicineExpression.ExpressionSpec(specParams))
    {
        IsPagingEnabled = false;
    }
}

public class MedicineExpression
{
    public static Expression<Func<Medicine, bool>> ExpressionSpec(
        GetAllMedicineQuery specParams)
    {
        return x => true;
    }
}
