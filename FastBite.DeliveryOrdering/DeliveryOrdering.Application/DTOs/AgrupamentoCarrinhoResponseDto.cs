using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class AgrupamentoCarrinhoResponseDto
    {
        public List<ComboPreviewDto> Combos { get; set; } = new();
        public List<ItemAvulsoPreviewDto> Avulsos { get; set; } = new();
        public List<CreateOrderItemDto> Items { get; set; } = new();  // lista plana normalizada para reenviar
        public decimal Subtotal { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal TaxaEntrega { get; set; }
        public decimal Total { get; set; }
        public List<ItemIndisponivelDto> Indisponiveis { get; set; } = new();
        public bool PodeFinalizar => Indisponiveis.Count == 0;
    }
}
