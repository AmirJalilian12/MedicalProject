using Application.DTOs.Contracts;
using Domain.Entities.OrderEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Orders.Queries.GetDeliveryMethods
{
    public class GetDeliveryMethodsQuery : IRequest<List<DeliveryMethod>>
    {

    }

    public class GetDeliveryMethodsQueryHandler : IRequestHandler<GetDeliveryMethodsQuery, List<DeliveryMethod>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetDeliveryMethodsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<DeliveryMethod>> Handle(GetDeliveryMethodsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ToListAsync(cancellationToken);
        }
    }
}
