using AltV.Net.Client.Async;
using Microsoft.Extensions.DependencyInjection;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource;

public sealed class ClientResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;

    public ClientResource()
    {
        var serviceCollection = new ServiceCollection();
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnStart()
    {
        // TODO: Add logging for startup
        serviceProvider.GetServices<IStartup>();
    }

    public override void OnStop() { }
}
