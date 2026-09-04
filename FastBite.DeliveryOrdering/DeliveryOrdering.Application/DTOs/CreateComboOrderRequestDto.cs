using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    /// <summary>
    /// DTO para criar um pedido com combos
    /// </summary>
    public class CreateComboOrderRequestDto
    {
        /// <summary>
        /// Lista de combos que o cliente deseja encomendar
        /// </summary>
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
