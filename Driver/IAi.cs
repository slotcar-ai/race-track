using System.Threading;
using System.Threading.Tasks;
using Scai.Driver.Data;

namespace Scai.Driver
{
    public interface IAi
    {
        Task<AiResult> RunAsync(TrackState state);

        Task<AiResult> RunAsync(TrackState state, CancellationToken token);
    }
}
