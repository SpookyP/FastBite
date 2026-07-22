using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Secret = Duende.IdentityServer.Models.Secret;

namespace FastBite.Identity.Api.Data
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),

                new IdentityResource("roles","Perfis de Utilizador", new[]{"role"})
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("MenuCatalog.api.full", "FastBite Food Catalog"),
                new ApiScope("DeliveryOrdering.api.full", "FastBite Food Ordering"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("fastbite.menu", "FastBite Menu API")
                {
                    Scopes = { "MenuCatalog.api.full" },
                },
                new ApiResource("fastbite.order", "FastBite Order API")
                {
                    Scopes = { "DeliveryOrdering.api.full" },
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[] {
                new Client{
                    ClientId = "fastbite.frontend",
                    ClientName = "FastBite Online",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    ClientSecrets = { new Secret("FastBite@!CHavEsUpErsEcrEtA".Sha256())},

                    RedirectUris = {"https://localhost:7280/signin-oidc"},
                    PostLogoutRedirectUris = {"https://localhost:7280/signout-callback-oidc"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "MenuCatalog.api.full",
                        "DeliveryOrdering.api.full"
                    },

                    AllowOfflineAccess = true,
                },
                new Client
                {
                    ClientId = "api.client.test",
                    ClientName = "Postman Test Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("FastBite@!CHavEsUpErsEcrEtA".Sha256()) },
                    AllowedScopes =
                    {
                        "MenuCatalog.api.full",
                        "DeliveryOrdering.api.full"
                    }
                }
        };
    }
}
