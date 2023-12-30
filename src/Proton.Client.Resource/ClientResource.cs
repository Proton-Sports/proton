using AltV.Net.Client.Async;
using Microsoft.Extensions.DependencyInjection;

namespace Proton.Server.Resource;

public sealed class ClientResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;

    public ClientResource()
    {
        var serviceCollection = new ServiceCollection();
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnStart() { }

    public override void OnStop() { }
}
