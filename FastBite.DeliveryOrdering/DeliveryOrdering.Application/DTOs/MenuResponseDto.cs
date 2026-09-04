using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class MenuResponseDto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public decimal PrecoBase { get; set; }

        public int LimiteDiario { get; set; }

        public string Categoria { get; set; }
    }
}
