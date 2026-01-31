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
        private readonly string _vaultUri;
        private readonly Lazy<SecretClient> _secretClient;

        public AzureKeyVaultSecretProvider(string? vaultUri)
        {
            if (string.IsNullOrWhiteSpace(vaultUri))
            {
                throw new ArgumentException("Vault URI is required.", nameof(vaultUri));
            }

            if (!Uri.TryCreate(vaultUri, UriKind.Absolute, out Uri? parsedVaultUri))
            {
                throw new ArgumentException("Vault URI must be an absolute URI.", nameof(vaultUri));
            }

            _vaultUri = parsedVaultUri.ToString();
            _secretClient = new Lazy<SecretClient>(CreateSecretClient, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task<string?> GetSecretAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Secret name is required.", nameof(name));
            }

            SecretClient secretClient = _secretClient.Value;

            try
            {
                Response<KeyVaultSecret> response = await secretClient.GetSecretAsync(name, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                return response.Value.Value;
            }
            catch (AuthenticationFailedException ex)
            {
                throw new InvalidOperationException("Failed to authenticate to Key Vault. Verify DefaultAzureCredential configuration.", ex);
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException($"Failed to retrieve secret '{name}' from Key Vault (status {ex.Status}).", ex);
            }
        }

        private SecretClient CreateSecretClient()
        {
            Uri vaultUri = new Uri(_vaultUri, UriKind.Absolute);

            return new SecretClient(vaultUri, new DefaultAzureCredential());
        }
    }
}
