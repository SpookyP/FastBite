using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class ComboPreviewDto
    {
        public int PratoId { get; set; }
        public string PratoNome { get; set; } = string.Empty;
        public int AcompanhamentoId { get; set; }
        public string AcompanhamentoNome { get; set; } = string.Empty;
        public int BebidaId { get; set; }
        public string BebidaNome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PrecoOriginalUnitario { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DescontoUnitario { get; set; }
        public decimal TotalLinha { get; set; }
    }
}
