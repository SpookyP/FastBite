using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class PagamentoDto
    {
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal TaxaEntrega { get; set; }
    }
}
