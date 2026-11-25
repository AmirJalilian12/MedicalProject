using Application.Interfaces;
using Domain.Entities.BasketEntities;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Basket.Commands
{
    public class DeleteItemFromBasketCommand : IRequest<CustomerBasket>
    {
        public string BasketId { get; set; }
        public int ItemId { get; set; }
        public DeleteItemFromBasketCommand(string basketId , int itemId)
        {
            BasketId = basketId;
            ItemId = itemId;
        }
    }
    public class DeleteItemFromBasketCommandHandler : IRequestHandler<DeleteItemFromBasketCommand, CustomerBasket>
    {
        private readonly IBasketRepository _basketRepository;
        public DeleteItemFromBasketCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }
        public async Task<CustomerBasket> Handle(DeleteItemFromBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetBasketAsync(request.BasketId);
            if (basket == null) throw new NotFoundException("سبد خرید شما یافت نشد");
            basket.Items = basket.Items.Where(x => x.Id != request.ItemId).ToList();
            if (basket.Items.Count == 0) await _basketRepository.DeleteBasketAsync(request.BasketId);
            else await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }
    }
}
