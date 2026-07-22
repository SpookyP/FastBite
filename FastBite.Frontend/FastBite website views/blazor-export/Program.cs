using FastBite.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Blazor ────────────────────────────────────────────────────────────────────
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();   // use AddInteractiveWebAssemblyComponents() for WASM

// ── State (Scoped = per user session) ────────────────────────────────────────
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<CartState>();

// ── Identity API ──────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<IdentityService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiUrls:Identity"]
        ?? throw new InvalidOperationException("ApiUrls:Identity not configured"));
});

// ── Catalog API ───────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<CatalogService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiUrls:Catalog"]
        ?? throw new InvalidOperationException("ApiUrls:Catalog not configured"));
});

// ── Ordering API ──────────────────────────────────────────────────────────────
builder.Services.AddHttpClient<OrderingService>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiUrls:Ordering"]
        ?? throw new InvalidOperationException("ApiUrls:Ordering not configured"));
});

// ── JWT handler (adds Authorization header automatically) ────────────────────
// Uncomment and wire AuthState.Token into a DelegatingHandler if your APIs
// require bearer tokens on every request.
// builder.Services.AddScoped<JwtAuthHandler>();
// builder.Services.AddHttpClient<CatalogService>(...).AddHttpMessageHandler<JwtAuthHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
