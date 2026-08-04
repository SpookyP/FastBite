using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Application.DTOs
{
    public class MenuComboResponseDto
    {
        public int Id { get; set; }
        public string Nome {  get; set; } = string.Empty;
        public string PratoNome {  get; set; } = string.Empty;
        public string AcompanhamentoNome { get; set; } = string.Empty;
        public string BebidaNome { get; set; } = string.Empty;

        public decimal PrecoFinal;
    }
}
