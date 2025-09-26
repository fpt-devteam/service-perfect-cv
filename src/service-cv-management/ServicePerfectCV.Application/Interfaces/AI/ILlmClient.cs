using System.Threading;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces.AI
{
    public interface ILlmClient
    {
        Task<string> CompleteAsync(string inputJson, CancellationToken cancellationToken);
    }
}
