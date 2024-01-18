using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Elements.Args;
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Adapters;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;

namespace Proton.Server.Resource;

public sealed class ClientResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;

    public ClientResource()
    {
        var serviceCollection = new ServiceCollection()
            .AddInfrastructure()
            .AddNoClips()
            .AddRaces();
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnStart()
    {
        Alt.RegisterMValueAdapter(SharedRaceCreatorDataMValueAdapter.Instance);
        Alt.RegisterMValueAdapter(RaceMapDto.Adapter.Instance);
        Alt.RegisterMValueAdapter(DefaultMValueAdapters.GetArrayAdapter(RaceMapDto.Adapter.Instance));

        // TODO: Add logging for startup
        serviceProvider.GetServices<IStartup>();
    }

    public override void OnStop() { }

    public override IBaseObjectFactory<IWebView> GetWebViewFactory()
    {
        return serviceProvider.GetRequiredService<IUiViewFactory>();
    }
}
