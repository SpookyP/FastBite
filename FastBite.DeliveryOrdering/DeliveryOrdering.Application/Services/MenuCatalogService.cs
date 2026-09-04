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

        public async Task<decimal> VerificarDisponibilidadeComboAsync(int pratoId, int acompanhamentoId, int bebidaId, int quantity)
        {
            try
            {
                AdicionarTokenAoHeader();

                // Chamar o endpoint do MenuCatalog que valida e retorna o preço do combo
                // Você pode usar um DTO simples para isto
                var response = await _httpClient.GetAsync($"api/Menus/combo?pratoId={pratoId}&acompanhamentoId={acompanhamentoId}&bebidaId={bebidaId}");

                if (!response.IsSuccessStatusCode) return 0;

                var comboResponse = await response.Content.ReadFromJsonAsync<dynamic>();

                // Retornar o preço final do combo
                return comboResponse?.PrecoFinal ?? 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}