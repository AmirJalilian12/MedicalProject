using Application.Interfaces;
using Domain.Entities.BasketEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Basket.Queries.GetBasketById
{
    public class GetBasketByIdQuery : IRequest<CustomerBasket>
    {
        public GetBasketByIdQuery(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
    }
    public class GetBasketByIdQueryHandler : IRequestHandler<GetBasketByIdQuery, CustomerBasket>
    {
        private readonly IBasketRepository _basketRepository;
        public GetBasketByIdQueryHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }
        public async Task<CustomerBasket> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
        {
            return await _basketRepository.GetBasketAsync(request.Id);
        }
    }
}
