using Application.Interfaces;
using Domain.Entities.BasketEntities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Basket.Commands
{
    public class UpdateBasketCommand : IRequest<CustomerBasket>
    {
        public UpdateBasketCommand(CustomerBasket customerBasket)
        {
            CustomerBasket = customerBasket;
        }
        public CustomerBasket CustomerBasket { get; set; }
    }
    public class UpdateBasketCommandHandler : IRequestHandler<UpdateBasketCommand, CustomerBasket>
    {
        private readonly IBasketRepository _basketRepository;
        public UpdateBasketCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;   
        }
        public async Task<CustomerBasket> Handle(UpdateBasketCommand request, CancellationToken cancellationToken)
        {
            return await _basketRepository.UpdateBasketAsync(request.CustomerBasket);
        }
    }
}
