using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class OrderItemResponseDto
    {
        public string Type { get; set; } = string.Empty;
        public string DescricaoItem { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? AcompanhamentoNome { get; set; }
        public string? BebidaNome { get; set; }
    }
}
