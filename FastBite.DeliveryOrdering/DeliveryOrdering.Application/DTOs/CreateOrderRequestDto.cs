using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    /// <summary>
    /// Corpo do pedido enviado pelo cliente ao criar uma nova encomenda (POST /order).
    /// </summary>
    public class CreateOrderRequestDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
