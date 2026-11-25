using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.ProductEntities;
using Application.DTOs.Contracts;

namespace Application.Features.ProductType.Queries.GetAll
{
    public class GetAllProductTypeQuery : IRequest<IEnumerable<Domain.Entities.ProductEntities.ProductType>>
    {

    }
    public class GetAllProductTypeQueryHandler : IRequestHandler<GetAllProductTypeQuery, IEnumerable<Domain.Entities.ProductEntities.ProductType>>
    {
        private readonly IUnitOfWork  _uow;
        public GetAllProductTypeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<Domain.Entities.ProductEntities.ProductType>> Handle(GetAllProductTypeQuery request, CancellationToken cancellationToken)
        {
            return await _uow.Repository<Domain.Entities.ProductEntities.ProductType>().GetAllAsync(cancellationToken);
        }
    }
}
