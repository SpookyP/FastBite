using System.ComponentModel.DataAnnotations;

namespace MenuCatalog.Application.DTOs
{
    public class MenuCreateEditDto
    {
        [Required(ErrorMessage = "O nome do menu é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição do menu é obrigatória.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A categoria do menu é obrigatória.")]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "Os alérgenos do menu são obrigatórios.")]
        public string Alergenios { get; set; }


        [Required(ErrorMessage = "O preço base do menu é obrigatório.")]
        [Range(0.01, 1000, ErrorMessage = "O preço base deve ser maior que zero.")]
        public decimal PrecoBase { get; set; }

        [Required(ErrorMessage = "O número limite diário do menu é obrigatório.")]
        [Range(1, 100, ErrorMessage = "O limite diário deve ser maior que zero.")]
        public int LimiteDiario { get; set; }
    }
}
