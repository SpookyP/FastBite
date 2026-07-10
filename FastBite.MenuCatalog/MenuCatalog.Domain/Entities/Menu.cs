using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCatalog.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nome {  get; set; } = string.Empty;
        public string Descricao {  get; set; } = string.Empty;
        public string Categoria {  get; set; } = string.Empty;
        public string Alergenios {  get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public int LimiteDiario { get; set; }
    }
}
