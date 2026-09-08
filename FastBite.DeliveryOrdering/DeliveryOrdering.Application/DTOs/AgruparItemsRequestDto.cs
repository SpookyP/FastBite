using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    /// <summary>
    /// Pedido de pré-visualização (POST /api/orders/agrupar Sem persistência, sem stock.
    /// </summary>
    public class AgruparItemsRequestDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();
        public decimal? TaxaEntrega { get; set; }   // opcional
    }
}
