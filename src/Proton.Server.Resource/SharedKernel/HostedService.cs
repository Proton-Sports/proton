using Microsoft.Extensions.Hosting;

namespace Proton.Server.Resource.SharedKernel;

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
