using System.Threading;
using System.Threading.Tasks;

namespace SecretsManagement.Core
{
    public interface ISecretProvider
    {
        Task<string?> GetSecretAsync(string name, CancellationToken cancellationToken);
    }
}
