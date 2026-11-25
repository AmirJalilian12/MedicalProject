using Application.DTOs.Orders;
using Domain.Entities.OrderEntities;
using Mapster;

namespace Application.Common.Mapping.Configurations
{
    public class OrderMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Order -> OrderDto
            config.NewConfig<Order, OrderDto>()
                .Map(
                    dest => dest.Status,
                    src => (int)src.OrderStatus
                )
                .Map(
                    dest => dest.Total,
                    src => src.GetOriginalTotal()
                )
                .Map(
                    dest => dest.Portal,
                    src => src.Portal
                );

            // OrderItem -> OrderItemDto
            config.NewConfig<OrderItem, OrderItemDto>()
                .Map(
                    dest => dest.ProductItemId,
                    src => src.ItemOrdered.ProductItemId
                )
                .Map(
                    dest => dest.ProductName,
                    src => src.ItemOrdered.ProductName
                )
                .Map(
                    dest => dest.ProductBrandName,
                    src => src.ItemOrdered.ProductBrandName
                )
                .Map(
                    dest => dest.ProductTypeName,
                    src => src.ItemOrdered.ProductTypeName
                )
                .Map(
                    dest => dest.PictureUrl,
                    src => src.ItemOrdered.PictureUrl
                );

            // Portal -> PortalDto
            config.NewConfig<Portal, PortalDto>();
        }
    }
}