using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace SecretsManagement.Core
{
    public class AzureKeyVaultSecretProvider : ISecretProvider
    {
        private readonly string? _vaultUri;
        private SecretClient? _secretClient;

        public AzureKeyVaultSecretProvider(string? vaultUri)
        {
            _vaultUri = vaultUri;
        }

        public async Task<string?> GetSecretAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Secret name is required.", nameof(name));
            }

            SecretClient secretClient = GetSecretClient();

            try
            {
                Response<KeyVaultSecret> response = await secretClient.GetSecretAsync(name, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return response.Value.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        private SecretClient GetSecretClient()
        {
            if (string.IsNullOrWhiteSpace(_vaultUri))
            {
                throw new InvalidOperationException("KeyVault:VaultUri configuration is required.");
            }

            return _secretClient ??= new SecretClient(new Uri(_vaultUri, UriKind.Absolute), new DefaultAzureCredential());
        }
    }
}
