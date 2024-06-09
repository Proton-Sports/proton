using Microsoft.Extensions.Hosting;

namespace Proton.Server.Resource.SharedKernel;

public abstract class HostedService : IHostedService
{
    public Task StartAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
