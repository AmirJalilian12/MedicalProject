using Application.DTOs.Contracts;
using Application.DTOs.Orders;
using Domain.Entities.OrderEntities;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Queries.GetOrdersForUser
{
    public class GetOrdersForUserQuery : IRequest<List<OrderDto>>
    {
    }

    public class GetOrdersForUserQueryHandler :
        IRequestHandler<GetOrdersForUserQuery, List<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentService _currentService;

        public GetOrdersForUserQueryHandler(
            ICurrentService currentService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _currentService = currentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersForUserQuery request,CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Repository<Order>()
                .Where(x => x.CreatedBy == _currentService.UserId)
                .Include(x => x.DeliveryMethod)
                .Include(x => x.Portal)
                .Include(x => x.ShipToAddress)
                .Include(x => x.OrderItems)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}