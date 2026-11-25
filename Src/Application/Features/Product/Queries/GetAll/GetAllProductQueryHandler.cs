using Application.DTOs.Contracts;
using Application.DTOs.Products;
using Application.Features.Product.Spec;
using Application.Wrappers;
using Domain.Entities.ProductEntities;
using Mapster;                    // ← به‌جای AutoMapper
using MediatR;
using Microsoft.Extensions.Configuration;   // ← برای خواندن appsettings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Product.Queries.GetAll
{
    public class GetAllProductQuery : RequestParametersBasic, IRequest<PaginationResponse<ProductDto>>, ICacheQuery
    {
        public int? TypeId { get; set; }
        public int? BrandId { get; set; }
        public int HoursSaveData => 1;
    }

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, PaginationResponse<ProductDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public GetAllProductQueryHandler(IUnitOfWork uow, IConfiguration configuration)
        {
            _uow = uow;
            _configuration = configuration;
        }

        public async Task<PaginationResponse<ProductDto>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetProductSpec(request);
            var result = await _uow.Repository<Domain.Entities.ProductEntities.Product>().ListAnySpec(spec, cancellationToken);
            var count = await _uow.Repository<Domain.Entities.ProductEntities.Product>().CountAsyncSpec(new ProductsCountSpec(request), cancellationToken);

            // ✅ Mapster — به‌جای _mapper.Map<...>
            var model = result.Adapt<List<ProductDto>>();
            return new PaginationResponse<ProductDto>(request.PageIndex, request.PageSize, count, model);
        }
    }
}
