using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class EntregaDto
    {
        public string NomeCompleto { get; set; } = string.Empty;
        public string ContactoTelefonico { get; set; } = string.Empty;
        public string Morada { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
    }
    }
}
