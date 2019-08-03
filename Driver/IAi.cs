using Scai.Driver.Data;

namespace Scai.Driver
{
    public interface IAi
    {
        AiResult Run(TrackState state);
    }
}
