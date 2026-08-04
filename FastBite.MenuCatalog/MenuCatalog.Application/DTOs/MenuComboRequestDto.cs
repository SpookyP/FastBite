using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Application.DTOs
{
    public class MenuComboRequestDto
    {
        [Required(ErrorMessage = "O Id do Prato é obrigatório")]
        public int PratoId { get; set; }

        [Required(ErrorMessage = "O Id do Acompanhamento é obrigatório")]
        public int AcompanhamentoId { get; set; }

        [Required(ErrorMessage = "O Id da Bebida é obrigatório")]
        public int BebidaId { get; set; }
    }
}
