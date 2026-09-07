using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class CreateOrderRequestDto
    {
        public List<CreateOrderItemDto> Items { get; set; } = new();
        public EntregaDto Entrega { get; set; } = new();
        public PagamentoDto Pagamento { get; set; } = new();
    }
}
