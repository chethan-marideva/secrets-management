using Microsoft.AspNetCore.Authentication;

namespace SecretsManagement.WebApi
{
    public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string HeaderName = "X-Api-Key";

        public string? ApiKey { get; set; }
    }
}
