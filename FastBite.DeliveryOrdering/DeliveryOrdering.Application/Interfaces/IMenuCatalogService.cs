using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryOrdering.Application.Services;

namespace DeliveryOrdering.Application.Interfaces
{
    public interface IMenuCatalogService
    {
        Task<CatalogItemResponse?> ValidarItemNoCatalogoAsync(int productId, int quantity);    // Método para validar se o item está disponível no catálogo
        Task<CatalogItemResponse?> ObterPrecoDoItemAsync(int productId);    // Método para obter o preço do item no catálogo    
    }
}
