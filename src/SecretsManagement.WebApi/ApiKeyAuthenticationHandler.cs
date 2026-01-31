using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SecretsManagement.WebApi
{
    public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        public const string SchemeName = "ApiKey";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(Options.ApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("API key authentication is not configured."));
            }

            if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var providedKey) ||
                string.IsNullOrWhiteSpace(providedKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            byte[] providedKeyBytes = Encoding.UTF8.GetBytes(providedKey.ToString());
            byte[] expectedKeyBytes = Encoding.UTF8.GetBytes(Options.ApiKey);

            if (providedKeyBytes.Length != expectedKeyBytes.Length ||
                !CryptographicOperations.FixedTimeEquals(providedKeyBytes, expectedKeyBytes))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
            }

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") }, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
