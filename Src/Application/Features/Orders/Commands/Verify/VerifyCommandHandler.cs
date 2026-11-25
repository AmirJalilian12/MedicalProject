using Application.DTOs.Contracts;
using Domain.Entities.OrderEntities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Commands.Verify
{
    public class VerifyCommand : IRequest<string>
    {
        public string Authority { get; set; }
        public string Status { get; set; }

        public VerifyCommand(string authority, string status)
        {
            Status = status;
            Authority = authority;
        }
    }

    public class VerifyCommandHandler : IRequestHandler<VerifyCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public VerifyCommandHandler(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(VerifyCommand request, CancellationToken cancellationToken)
        {
            // 1. یافتن سفارش با Authority
            var order = await _unitOfWork.Context.Set<Order>()
                .Include(x => x.DeliveryMethod)
                .Where(x => x.Authority == request.Authority)
                .SingleOrDefaultAsync(cancellationToken);

            if (order == null)
                throw new BadRequestException("سفارش مورد نظر شما پیدا نشد مجددا تلاش بفرمایید");

            // 2. یافتن رکورد پورتال مربوطه
            var portal = await _unitOfWork.Repository<Portal>()
                .Where(x => x.OrderId == order.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if (portal == null)
                throw new BadRequestException("در پرداخت شما مشکلی بوجود آمده است لطفا با پشتیبانی تماس بگیرید");

            // 3. در صورت انصراف کاربر در صفحه درگاه
            if (order.IsFinally && portal.Status == PaymentDataStatus.Success)
                return _configuration["Order:CallBackSuccess"];

            if (request.Status != "OK")
            {
                order.OrderStatus = OrderStatus.Canceled;
                portal.Status = PaymentDataStatus.Canceled;
                await _unitOfWork.Repository<Portal>().UpdateAsync(portal);
                await _unitOfWork.Save(cancellationToken);

                return _configuration["Order:CallBackCanceled"];
            }

            // 4. اعتبارسنجی پرداخت با سندباکس زرین‌پال V4
            var amount = portal.Amount;
            var (isVerified, refId) = await VerifyZarinpalSandboxAsync(amount, request.Authority);

            if (isVerified)
            {
                order.IsFinally = true;
                order.OrderStatus = OrderStatus.Pending;
                portal.ReferenceId = refId;
                portal.Status = PaymentDataStatus.Success;
                await _unitOfWork.Repository<Portal>().UpdateAsync(portal);
                await _unitOfWork.Save(cancellationToken);

                return _configuration["Order:CallBackSuccess"];
            }
            else
            {
                order.OrderStatus = OrderStatus.PaymentFailed;
                portal.Status = PaymentDataStatus.Failed;
                await _unitOfWork.Repository<Portal>().UpdateAsync(portal);
                await _unitOfWork.Save(cancellationToken);

                return _configuration["Order:CallBackFailed"];
            }
        }

        private async Task<(bool IsVerified, string RefId)> VerifyZarinpalSandboxAsync(long amount, string authority)
        {
            using var client = new HttpClient();

            var payload = new
            {
                merchant_id = "00000000-0000-0000-0000-000000000000",
                amount = amount,
                authority = authority
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("https://sandbox.zarinpal.com/pg/v4/payment/verify.json", jsonContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Object)
            {
                if (dataElement.TryGetProperty("code", out var code) && (code.GetInt32() == 100 || code.GetInt32() == 101))
                {
                    var refId = dataElement.GetProperty("ref_id").GetInt64().ToString();
                    return (true, refId);
                }
            }

            return (false, "0");
        }
    }
}
