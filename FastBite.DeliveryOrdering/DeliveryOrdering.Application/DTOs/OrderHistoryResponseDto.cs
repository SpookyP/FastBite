using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class OrderHistoryResponseDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int OrderType { get; set; }
        public string NomeCompleto { get; set; }
        public string ContactoTelefonico { get; set; }
        public string Morada { get; set; }
        public string CodigoPostal { get; set; }
        public string Cidade { get; set; }
        public string MetodoPagamento { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal Subtotal { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }
}
