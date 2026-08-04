using System;

namespace MenuCatalog.Domain.Entities
{
    public class MenuCombo
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public int PratoId { get; private set; }
        public Item Prato { get; private set; } = null!;
        public int AcompanhamentoId { get; private set; }
        public Item Acompanhamento { get; private set; } = null!;
        public int BebidaId { get; private set; }
        public Item Bebida { get; private set; } = null!;

        private MenuCombo() { } // O Entity Framework precisa de um construtor vazio para ler da base de dados

        // 1. CORREÇÃO: Parâmetros alterados para minúsculas (nome, prato, acompanhamento, bebida)
        public MenuCombo(string nome, Item prato, Item acompanhamento, Item bebida)
        {
            if (prato.Categoria != "Prato")
                throw new ArgumentException("O item principal tem de ser da categoria 'Prato'.");

            if (acompanhamento.Categoria != "Acompanhamento")
                throw new ArgumentException("O acompanhamento tem de ser da categoria 'Acompanhamento'.");

            if (bebida.Categoria != "Bebida")
                throw new ArgumentException("A bebida tem de ser da categoria 'Bebida'.");

            Nome = nome;
            Prato = prato;
            PratoId = prato.Id;

            Acompanhamento = acompanhamento;
            AcompanhamentoId = acompanhamento.Id;

            Bebida = bebida;
            BebidaId = bebida.Id;
        }
    }
}