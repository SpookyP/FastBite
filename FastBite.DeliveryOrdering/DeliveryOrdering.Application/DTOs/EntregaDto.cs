using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    /// <summary>
    /// DTO para dados de entrega
    /// </summary>
    public class EntregaDto
    {
        public string NomeCompleto { get; set; }
        public string ContactoTelefonico { get; set; }
        public string Morada { get; set; }
        public string CodigoPostal { get; set; }
        public string Cidade { get; set; }
    }
}
