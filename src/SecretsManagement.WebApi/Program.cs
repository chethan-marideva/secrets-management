using SecretsManagement.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ISecretProvider>(sp =>
    new AzureKeyVaultSecretProvider(sp.GetRequiredService<IConfiguration>()["KeyVault:VaultUri"]));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/secrets/{name}", async (string name, ISecretProvider secretProvider, CancellationToken cancellationToken) =>
{
    string? secret = await secretProvider.GetSecretAsync(name, cancellationToken);
    return string.IsNullOrWhiteSpace(secret)
        ? Results.NotFound("Secret not found.")
        : Results.Ok(secret);
})
.WithName("GetSecret")
.WithTags("Secrets");

app.Run();
