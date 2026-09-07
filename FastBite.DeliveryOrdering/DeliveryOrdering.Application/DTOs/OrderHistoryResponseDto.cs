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
        public decimal Subtotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public string NomeCompleto { get; set; } = string.Empty;
        public string ContactoTelefonico { get; set; } = string.Empty;
        public string Morada { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal TaxaEntrega { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
