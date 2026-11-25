using Application.DTOs.Contracts;
using Application.DTOs.Products;
using Application.Features.Product.Spec;
using Domain.Entities.ProductEntities;
using Domain.Exceptions;
using Mapster;
using MediatR;

namespace Application.Features.Product.Queries.Get
{
    public class GetProductQuery : IRequest<ProductDto>
    {
        public int Id { get; set; }

        public GetProductQuery(int id) => Id = id;
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductDto>
    {
        private readonly IUnitOfWork _uow;

        public GetProductQueryHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetProductSpec(request.Id);
            var model = await _uow.Repository<Domain.Entities.ProductEntities.Product>()
                                .GetEntityWithSpec(spec, cancellationToken);

            if (model == null)
                throw new NotFoundException($"محصولی با شناسه {request.Id} یافت نشد");

            return model.Adapt<ProductDto>();   // نگاشت در Mapster انجام می‌شود
        }
    }
}
