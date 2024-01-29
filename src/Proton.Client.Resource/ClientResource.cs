using AltV.Net.Client;
using AltV.Net.Client.Async;
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Authentication.Extentions;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource;

public sealed class ClientResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;

    public ClientResource()
    {
        var serviceCollection = new ServiceCollection();
        serviceProvider = serviceCollection
            .AddAuthentication()
            .BuildServiceProvider();
    }

    public override void OnStart()
    {
        // TODO: Add logging for startup
        serviceProvider.GetServices<IStartup>();
        Alt.Log("loaded!!!");
    }

    public override void OnStop() { }
}
