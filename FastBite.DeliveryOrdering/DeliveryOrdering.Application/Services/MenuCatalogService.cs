using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Services
{
    /// <summary>
    /// Contrato para consultar a MenuCatalog.API. A implementação (Infrastructure)
    /// é quem sabe que esta comunicação acontece via HTTP.
    /// </summary>
    public class MenuCatalogService : IMenuCatalogService
    {
        private readonly HttpClient _httpClient;

        // Construtor que recebe uma instância de HttpClient para fazer requisições HTTP
        public MenuCatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> VerificarDisponibilidadeAsync(int id, int quantidade)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Menus/VerDisponibilidade?id={id}&quantidade={quantidade}");
                if (!response.IsSuccessStatusCode) return false;
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<MenuResponseDto?> ObterMenuPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Menus/ObterPorId?id={id}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<MenuResponseDto>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
