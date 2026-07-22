using System.Net.Http.Json;
using FastBite.Models;

namespace FastBite.Services;

public class OrderingService(HttpClient http)
{
    // Replace with your real Ordering API base URL via appsettings.json
    // builder.Services.AddHttpClient<OrderingService>(c => c.BaseAddress = new Uri("https://your-ordering-api/api/"));

    public async Task<Order> CreateOrderAsync(CreateOrderPayload payload)
    {
        var response = await http.PostAsJsonAsync("orders", payload);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Order>())!;
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return (await http.GetFromJsonAsync<List<Order>>("orders"))!;
    }

    public async Task<Order?> GetOrderAsync(string id)
    {
        return await http.GetFromJsonAsync<Order>($"orders/{id}");
    }

    public async Task<Order> CancelOrderAsync(string id)
    {
        var response = await http.PostAsync($"orders/{id}/cancel", null);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Order>())!;
    }
}
