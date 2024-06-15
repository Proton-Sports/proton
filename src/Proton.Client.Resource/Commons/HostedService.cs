using Proton.Client.Resource.Commons.Abstractions;

namespace Proton.Client.Resource.Commons;

public abstract class HostedService : IHostedService
{
    public virtual Task StartAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public virtual Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
