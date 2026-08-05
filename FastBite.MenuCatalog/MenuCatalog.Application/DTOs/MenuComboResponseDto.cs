
namespace MenuCatalog.Application.DTOs
{
    public class MenuComboResponseDto
    {
        public string Nome {  get; set; } = string.Empty;
        public string PratoNome {  get; set; } = string.Empty;
        public string AcompanhamentoNome { get; set; } = string.Empty;
        public string BebidaNome { get; set; } = string.Empty;
        public decimal PrecoOriginal { get; set; }
        public decimal PrecoFinal { get; set; }
    }
}
