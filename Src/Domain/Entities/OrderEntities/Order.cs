using Domain.Entities.BaseEntities;
using Domain.Enums;

namespace Domain.Entities.OrderEntities
{
    public class Order : BaseAuditableEntity
    {
        public string BuyerPhoneNumber { get; set; }

        public decimal SubTotal { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public string? TrackingCode { get; set; }

        public Portal Portal { get; set; }

        public PortalType PortalType { get; set; } = PortalType.ZarrinPal;

        public bool IsFinally { get; set; } = false;

        public string Authority { get; set; }

        public decimal GetOriginalTotal()
        {
            // Prefer the immutable charge, even if delivery prices have since changed.
            return Portal != null ? Portal.Amount : SubTotal + (DeliveryMethod?.Price ?? 0);
        }

        public List<OrderItem> OrderItems { get; set; } = new();

        public ShipToAddress ShipToAddress { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        public User User { get; set; }
    }
}