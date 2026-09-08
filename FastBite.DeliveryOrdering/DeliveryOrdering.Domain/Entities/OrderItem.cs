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
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public OrderItemType Type { get; set; }
        public int ProductId { get; set; }
        public string DescricaoItem { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }

        public int? AcompanhamentoId { get; set; }
        public string? AcompanhamentoNome { get; set; }
        public int? BebidaId { get; set; }
        public string? BebidaNome { get; set; }
    }
    public enum OrderItemType
    {
        Avulso,
        Combo
    }
}
