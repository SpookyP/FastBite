using DeliveryOrdering.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Services
{
    public class MenuCatalogService : IMenuCatalogService
    {
        private readonly HttpClient _httpClient;

        // Construtor que recebe uma instância de HttpClient para fazer requisições HTTP
        public MenuCatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método para validar se o item está disponível no catálogo
        public async Task<CatalogItemResponse?> ValidarItemNoCatalogoAsync(Guid productId, int quantity)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/menu/{productId}/availability?quantity={quantity}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<CatalogItemResponse>(); // Tranforma uma resposta JSON em um objeto CatalogItemResponse
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Método para obter o preço do item no catálogo
        public async Task<CatalogItemResponse?> ObterPrecoDoItemAsync(Guid productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/menu/{productId}/price");
                if (!response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadFromJsonAsync<CatalogItemResponse>(); // Tranforma uma resposta JSON em um objeto CatalogItemResponse
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    public class CatalogItemResponse
    {
        public bool IsAvailable { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
