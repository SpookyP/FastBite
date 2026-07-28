using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    /// <summary>
    /// Representa a resposta do endpoint ObterPorId da MenuCatalog.API.
    /// Usado para obter o preço base de um prato ao calcular o total do pedido.
    /// </summary>
    public class MenuResponseDto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public decimal PrecoBase { get; set; }

        public int LimiteDiario { get; set; }
    }
}
