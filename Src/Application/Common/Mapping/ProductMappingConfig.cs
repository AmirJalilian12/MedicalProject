using Application.DTOs.Products;
using Domain.Entities.ProductEntities;
using Mapster;

namespace Application.Common.Mapping
{
    public class ProductMappingConfig
    {
        private readonly string _baseUrl;

        public ProductMappingConfig(
            string? backendUrl,
            string? productImagesPath)
        {
            _baseUrl =
                $"{(backendUrl ?? string.Empty).TrimEnd('/')}/" +
                $"{(productImagesPath ?? string.Empty).Trim('/')}/";
        }

        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDto>()
                .Map(
                    dest => dest.PictureUrl,
                    src => BuildUrl(src.PictureUrl)
                );
        }

        private string? BuildUrl(string? pictureUrl)
        {
            if (string.IsNullOrWhiteSpace(pictureUrl))
                return pictureUrl;

            // اگر URL کامل است، همان را برگردان
            if (pictureUrl.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase) ||
                pictureUrl.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase))
            {
                return pictureUrl;
            }

            return _baseUrl + pictureUrl.TrimStart('/');
        }
    }
}