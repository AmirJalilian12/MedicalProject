using Application.DTOs.Contracts;
using Application.DTOs.Orders;
using Domain.Entities.OrderEntities;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Orders.Queries.GetOrderByIdForUser
{
    public class GetOrderByIdForUserQuery : IRequest<OrderDto>
    {
        public int Id { get; set; }

        public GetOrderByIdForUserQuery(int id)
        {
            Id = id;
        }
    }

    public class GetOrderByIdForUserQueryHandler : IRequestHandler<GetOrderByIdForUserQuery, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentService _currentService;
        public GetOrderByIdForUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentService currentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentService = currentService;
        }
        public async Task<OrderDto> Handle(GetOrderByIdForUserQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<Order>().Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            return _mapper.Map<OrderDto>(order);
        }
    }
}
