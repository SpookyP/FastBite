using DeliveryOrdering.Application.DTOs;
using DeliveryOrdering.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DeliveryOrdering.Application.Services
{
    public class MenuCatalogService : IMenuCatalogService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuCatalogService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Copia o token JWT do pedido original (feito à DeliveryOrdering.API)
        // para a chamada que vamos fazer à MenuCatalog.API.
        private void AdicionarTokenAoHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    System.Net.Http.Headers.AuthenticationHeaderValue.Parse(token);
            }
        }

        public async Task<bool> VerificarDisponibilidadeAsync(int id, int quantidade)
        {
            try
            {
                AdicionarTokenAoHeader();
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
                AdicionarTokenAoHeader();
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