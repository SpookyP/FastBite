using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        // Chave forasteira para o Pedido
        public Guid OrderId { get; set; }

        // ID do prato (Para o Desenvolvedor A validar na Catalog.API)
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Propriedade de navegação
        public Order Order { get; set; }
    }
}
