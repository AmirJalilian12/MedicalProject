using Application.DTOs.Common;
using Domain.Entities.ProductEntities;
using Mapster;

namespace Application.DTOs.Products
{
    public class ProductDto : CommandDto, IRegister
    {   
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string? PictureUrl { get; set; }
        public int ProductBrandId { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductType { get; set; }
        public string ProductBrand { get; set; }

        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductDto>()
                .Map(d => d.ProductType, s => s.ProductType.Title)
                .Map(d => d.ProductBrand, s => s.ProductBrand.Title)
                .Map(d => d.PictureUrl, s =>
                    string.IsNullOrEmpty(s.PictureUrl)
                        ? null
                        : "http://localhost:9001/images/products/" + s.PictureUrl);
        }
    }
}
