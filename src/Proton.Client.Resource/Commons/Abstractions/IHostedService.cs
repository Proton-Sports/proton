namespace Proton.Client.Resource.Commons.Abstractions;

public interface IHostedService
{
    Task StartAsync(CancellationToken ct);
    Task StopAsync(CancellationToken ct);
}
