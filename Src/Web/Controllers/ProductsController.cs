using Application.DTOs.Products;
using Application.Features.Product.Queries.Get;
using Application.Features.Product.Queries.GetAll;
using Application.Features.ProductBrand.Queries.GetAll;
using Application.Features.ProductType.Queries.GetAll;
using Domain.Entities.ProductEntities;
using Microsoft.AspNetCore.Mvc;
using Web.Common;

namespace Web.Controllers
{
    public class ProductsController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get([FromQuery] GetAllProductQuery request , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request,cancellationToken));
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> Get([FromRoute]int id , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetProductQuery(id),cancellationToken));
        }
        [HttpGet("GetAllProductBrand")]
        public async Task<ActionResult<ProductBrand>>GetAllProductBrand(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetAllProductBrandQuery(),cancellationToken));
        }
        [HttpGet("GetAllProductType")]
        public async Task<ActionResult<ProductType>> GetAllProductType(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetAllProductTypeQuery(), cancellationToken));
        }
    }
}
