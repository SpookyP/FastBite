namespace MenuCatalog.Application.DTOs
{
    public class MenuResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Categoria { get; set; }

        public string Alergenios { get; set; }

        public decimal PrecoBase { get; set; }

        public int LimiteDiario { get; set; }
    }
}
