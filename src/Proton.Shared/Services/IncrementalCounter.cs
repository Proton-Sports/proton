using Proton.Shared.Interfaces;

namespace Proton.Shared.Services;

public sealed class IncrementalCounter : IIncrementalCounter
{
    private long count = 0;

    public long GetNext()
    {
        return Interlocked.Increment(ref count);
    }
}
