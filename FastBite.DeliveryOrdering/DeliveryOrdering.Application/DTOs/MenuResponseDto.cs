using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.DTOs
{
    public class MenuResponseDto
    {
        public int id { get; set; }

        public string nome { get; set; }

        public decimal PrecoBase { get; set; }

        public int LimiteDiario { get; set; }
    }
}
