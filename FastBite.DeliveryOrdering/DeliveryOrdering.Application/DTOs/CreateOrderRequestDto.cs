using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class CreateOrderRequestDto
    {
        /// <summary>
        /// Tipo de pedido: "items" para itens avulsos ou "combo" para combos
        /// </summary>
        public string OrderType { get; set; } = "items"; // Padrão é itens avulsos

        /// <summary>
        /// Uma lista com os pratos/itens que o cliente quer encomendar
        /// </summary>
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
