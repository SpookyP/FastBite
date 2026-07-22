using System.Net.Http.Json;
using FastBite.Models;

namespace FastBite.Services;

public class CatalogService(HttpClient http)
{
    // Replace with your real Catalog API base URL via appsettings.json
    // builder.Services.AddHttpClient<CatalogService>(c => c.BaseAddress = new Uri("https://your-catalog-api/api/"));

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return (await http.GetFromJsonAsync<List<Category>>("categories"))!;
    }

    public async Task<List<MenuItem>> GetItemsAsync(string? categoryId = null, string? search = null)
    {
        var query = new List<string>();
        if (!string.IsNullOrEmpty(categoryId) && categoryId != "all")
            query.Add($"category={Uri.EscapeDataString(categoryId)}");
        if (!string.IsNullOrEmpty(search))
            query.Add($"search={Uri.EscapeDataString(search)}");

        var url = query.Count > 0 ? $"items?{string.Join("&", query)}" : "items";
        return (await http.GetFromJsonAsync<List<MenuItem>>(url))!;
    }

    public async Task<MenuItem?> GetItemAsync(string id)
    {
        return await http.GetFromJsonAsync<MenuItem>($"items/{id}");
    }
}
