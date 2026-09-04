using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class CreateOrderItemDto
    {
        /// <summary>
        /// O ID do prato/item que vem do catálogo (ou PratoId para combos)
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Para combos: ID do acompanhamento
        /// </summary>
        public int? AcompanhamentoId { get; set; }

        /// <summary>
        /// Para combos: ID da bebida
        /// </summary>
        public int? BebidaId { get; set; }

        /// <summary>
        /// Quantidade desejada
        /// </summary>
        public int Quantity { get; set; }
    }
}
