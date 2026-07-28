using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Entities
{
    /// <summary>
    /// Estados possíveis de um pedido, do momento da criação até à entrega.
    /// </summary>
    public enum OrderStatus
    {
        Pendente,   
        APreparar,
        ACaminho,
        Entregue
    }
}
