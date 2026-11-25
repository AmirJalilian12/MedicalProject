using Domain.Entities.OrderEntities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Orders
{
    public class OrderItemDto : IMapFrom<OrderItem>
    {
        public int ProductItemId { get; set; }
        public string ProductName { get; set; }
        public string ProductBrandName { get; set; }
        public string ProductTypeName { get; set; }
        public string PictureUrl { get; set; }
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
