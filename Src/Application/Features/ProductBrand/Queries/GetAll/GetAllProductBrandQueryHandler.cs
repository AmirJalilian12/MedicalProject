using Application.DTOs.Contracts;
using Domain.Entities.ProductEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ProductBrand.Queries.GetAll
{
    public class GetAllProductBrandQuery : IRequest<IEnumerable<Domain.Entities.ProductEntities.ProductBrand>>
    {

    }
    public class GetAllProductBrandQueryHandler : IRequestHandler<GetAllProductBrandQuery, IEnumerable<Domain.Entities.ProductEntities.ProductBrand>>
    {
        private readonly IUnitOfWork _uow;
        public GetAllProductBrandQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<Domain.Entities.ProductEntities.ProductBrand>> Handle(GetAllProductBrandQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Repository<Domain.Entities.ProductEntities.ProductBrand>().GetAllAsync(cancellationToken);
        }
    }
}
