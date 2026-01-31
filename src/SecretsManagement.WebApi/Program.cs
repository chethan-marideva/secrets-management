using Microsoft.AspNetCore.Authentication;
using SecretsManagement.Core;
using SecretsManagement.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
string? vaultUri = builder.Configuration["KeyVault:VaultUri"];
if (string.IsNullOrWhiteSpace(vaultUri))
{
    throw new InvalidOperationException("KeyVault:VaultUri configuration is required.");
}
string? apiKey = builder.Configuration["Authentication:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("Authentication:ApiKey configuration is required.");
}
builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName,
        options => options.ApiKey = apiKey);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<ISecretProvider>(sp =>
    new AzureKeyVaultSecretProvider(vaultUri));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapGet("/secrets/{name}", async (string name, ISecretProvider secretProvider, CancellationToken cancellationToken) =>
{
    string? secret = await secretProvider.GetSecretAsync(name, cancellationToken);
    return string.IsNullOrWhiteSpace(secret)
        ? Results.NotFound("Secret not found.")
        : Results.Ok(secret);
})
.WithName("GetSecret")
.WithTags("Secrets")
.RequireAuthorization();

app.Run();
