using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class CreateOrderItemDto
    {
        // O ID do prato que vem do catálogo
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
