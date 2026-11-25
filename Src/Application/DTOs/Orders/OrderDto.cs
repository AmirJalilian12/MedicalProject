using Application.DTOs.Common;
using Domain.Entities.OrderEntities;
using Domain.Enums;
using Mapster;

namespace Application.DTOs.Orders
{
    public class OrderDto : BaseAuditableDtoResponse, IMapFrom<Order>
    {
        public string BuyerPhoneNumber { get; set; }

        public decimal SubTotal { get; set; }

        public string? TrackingCode { get; set; }

        public decimal Total { get; set; }

        public bool IsFinally { get; set; }

        // به جای Entity از DTO استفاده می‌کنیم
        public PortalDto? Portal { get; set; }

        public PortalType PortalType { get; set; }

        public string Authority { get; set; }

        public string Link { get; set; }

        public int Status { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public ShipToAddress ShipToAddress { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }
    }
}