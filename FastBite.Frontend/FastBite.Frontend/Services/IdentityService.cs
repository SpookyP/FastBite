using System.Net.Http.Json;
using FastBite.Models;

namespace FastBite.Services;

public class IdentityService(HttpClient http)
{
    // Replace with your real Identity API base URL via appsettings.json
    // builder.Services.AddHttpClient<IdentityService>(c => c.BaseAddress = new Uri("https://your-identity-api/api/"));

    public async Task<AuthResponse> LoginAsync(LoginPayload payload)
    {
        var response = await http.PostAsJsonAsync("auth/login", payload);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterPayload payload)
    {
        var response = await http.PostAsJsonAsync("auth/register", payload);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }

    public async Task<User> MeAsync()
    {
        return (await http.GetFromJsonAsync<User>("auth/me"))!;
    }
}
