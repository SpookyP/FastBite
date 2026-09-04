using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Domain.Entities
{
    /// <summary>
    /// Enum para distinguir o tipo de pedido
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Pedido com itens avulsos
        /// </summary>
        Avulso = 0,

        /// <summary>
        /// Pedido com combos
        /// </summary>
        Combo = 1
    }
}
