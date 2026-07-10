using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Entities
{
    public enum OrderStatus // Enum - Valores fixos
    {
        Pendente,   // Estado inicial do pedido
        APreparar,
        ACaminho,
        Entregue
    }
}
