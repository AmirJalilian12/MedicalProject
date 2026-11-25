using Application.DTOs.Contracts;
using Domain.Entities.OrderEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Orders.Queries.GetDeliveryMethodById
{
    public class GetDeliveryMethodByIdQuery : IRequest<DeliveryMethod>
    {
        public int Id { get; set; }
        public GetDeliveryMethodByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetDeliveryMethodByIdQueryHandler : IRequestHandler<GetDeliveryMethodByIdQuery, DeliveryMethod>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetDeliveryMethodByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<DeliveryMethod> Handle(GetDeliveryMethodByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Repository<DeliveryMethod>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
