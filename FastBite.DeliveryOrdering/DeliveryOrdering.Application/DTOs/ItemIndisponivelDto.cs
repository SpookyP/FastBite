using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class ItemIndisponivelDto
    {
        public int ProductId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int QuantidadePedida { get; set; }
    }
}
