using System.ComponentModel.DataAnnotations;

namespace MenuCatalog.Application.DTOs
{
    public class MenuComboCreateDto
    {
        [Required(ErrorMessage = "O Nome do Prato é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Id do Prato é obrigatório")]
        public int PratoId { get; set; }

        [Required(ErrorMessage = "O Id do Acompanhamento é obrigatório")]
        public int AcompanhamentoId { get; set; }

        [Required(ErrorMessage = "O Id da Bebida é obrigatório")]
        public int BebidaId { get; set; }
    }
}
