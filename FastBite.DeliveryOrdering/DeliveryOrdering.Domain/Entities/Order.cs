using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Subtotal { get; set; }
        public OrderStatus Status { get; set; }

        public string NomeCompleto { get; set; } = string.Empty;
        public string ContactoTelefonico { get; set; } = string.Empty;
        public string Morada { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal TaxaEntrega { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
