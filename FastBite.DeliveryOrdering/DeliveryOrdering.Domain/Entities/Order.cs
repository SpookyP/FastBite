using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; } // Identificador único do pedido
        public string UserId { get; set; } // Identificador do usuário que fez o pedido
        public DateTime OrderDate { get; set; } // Data e hora em que o pedido foi feito
        public decimal TotalAmount { get; set; } // Valor total do pedido
        public OrderStatus Status { get; set; } // Status atual do pedido (Pendente, APreparar, ACaminho, Entregue)
        public OrderType OrderType { get; set; } = OrderType.Avulso;
        public string NomeCompleto { get; set; }
        public string ContactoTelefonico { get; set; }
        public string Morada { get; set; }
        public string CodigoPostal { get; set; }
        public string Cidade { get; set; }
        public string MetodoPagamento { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal Subtotal { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>(); // Lista de itens do pedido
    }
}
