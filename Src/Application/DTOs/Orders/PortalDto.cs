using Domain.Entities.OrderEntities;
using Domain.Enums;
using Mapster;

namespace Application.DTOs.Orders
{
    public class PortalDto : IMapFrom<Portal>
    {
        public int OrderId { get; set; }

        public PortalType Gateway { get; set; }

        public PaymentDataStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public long Amount { get; set; }

        public string? ReferenceId { get; set; }
    }
}