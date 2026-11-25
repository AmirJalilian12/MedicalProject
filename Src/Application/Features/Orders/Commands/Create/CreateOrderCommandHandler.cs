using Application.DTOs.Contracts;
using Application.DTOs.Orders;
using Application.Interfaces;
using Domain.Entities.BasketEntities;
using Domain.Entities.OrderEntities;
using Domain.Enums;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Commands.Create
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public PortalType PortalType { get; set; } = PortalType.ZarrinPal;
        public ShipToAddress ShipToAddress { get; set; }
    }

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IConfiguration _configuration;
        private readonly ICurrentService _currentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBasketRepository basketRepository,
            IConfiguration configuration,
            ICurrentService currentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _basketRepository = basketRepository;
            _configuration = configuration;
            _currentService = currentService;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. دریافت سبد خرید
            var basket = await _basketRepository.GetBasketAsync(request.BasketId);

            if (basket == null)
            {
                throw new BadRequestException(
                    $"سبد خرید در Redis پیدا نشد. BasketId: {request.BasketId}"
                );
            }

            // 2. دریافت شیوه ارسال
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                .GetByIdAsync(request.DeliveryMethodId, cancellationToken);

            // 3. محاسبه مبلغ و ارتباط با سندباکس زرین‌پال (REST V4)
            var amount = (int)basket.CalculateOriginalPrice();
            var (authority, paymentLink) = await RequestZarinpalSandboxAsync(amount, request.BuyerPhoneNumber);

            // 4 & 5. ایجاد سفارش
            var result = await CreateOrder(request, cancellationToken, basket, deliveryMethod, authority);

            // 6. حذف سبد خرید
            await _basketRepository.DeleteBasketAsync(basket.Id);

            // 7. ثبت در جدول Portal
            var portal = new Portal(result.Id, result.PortalType, PaymentDataStatus.Pending, amount, null);
            await _unitOfWork.Repository<Portal>().AddAsync(portal, cancellationToken);
            await _unitOfWork.Save(cancellationToken);

            // 8. مپ خروجی و قرار دادن لینک درگاه
            var model = _mapper.Map<OrderDto>(result);
            model.Link = paymentLink;

            return model;
        }

        private async Task<(string Authority, string PaymentLink)> RequestZarinpalSandboxAsync(int amount, string phone)
        {
            using var client = new HttpClient();

            var payload = new
            {
                merchant_id = "00000000-0000-0000-0000-000000000000",
                amount = amount,
                description = "سفارش تستی فروشگاه",
                callback_url = _configuration["Order:CallBack"],
                metadata = new
                {
                    mobile = phone
                }
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("https://sandbox.zarinpal.com/pg/v4/payment/request.json", jsonContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind != JsonValueKind.Null)
            {
                if (dataElement.TryGetProperty("code", out var code) && (code.GetInt32() == 100 || code.GetInt32() == 101))
                {
                    var authority = dataElement.GetProperty("authority").GetString();
                    var link = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}";
                    return (authority, link);
                }
            }

            throw new BadRequestException($"خطا در ارتباط با سندباکس زرین‌پال: {responseBody}");
        }

        private async Task<Order> CreateOrder(
            CreateOrderCommand request,
            CancellationToken cancellationToken,
            CustomerBasket basket,
            DeliveryMethod deliveryMethod,
            string authority)
        {
            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var itemOrder = new ProductItemOrdered(item.Id, item.Product, item.Brand, item.Type, item.PictureUrl);
                orderItems.Add(new OrderItem()
                {
                    ItemOrdered = itemOrder,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            var order = new Order()
            {
                BuyerPhoneNumber = request.BuyerPhoneNumber,
                ShipToAddress = request.ShipToAddress,
                DeliveryMethod = deliveryMethod,
                OrderItems = orderItems,
                SubTotal = basket.CalculateOriginalPrice(),
                PortalType = request.PortalType,
                Authority = authority,
                CreatedBy = _currentService.UserId
            };

            var result = await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
            if (result == null)
                throw new BadRequestException("سفارش شما با شکست روبرو شد لطفا مجدد تلاش کنید");

            await _unitOfWork.Save(cancellationToken);
            return result;
        }
    }
}
