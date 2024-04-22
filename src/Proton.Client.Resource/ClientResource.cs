using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Elements.Args;
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Adapters;
using Proton.Shared.Dtos;
using Proton.Client.Resource.Authentication.Extentions;
using Proton.Client.Resource.CharacterCreator.Extensions;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;
using AltV.Net;
using Proton.Client.Resource.Utils.Extentions;
using Proton.Shared.Extensions;

namespace Proton.Server.Resource;

public sealed class ClientResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;

    public ClientResource()
    {
        var serviceCollection = new ServiceCollection()
            .AddInfrastructure()
            .AddNoClips()
            .AddAuthentication()
            .AddRaceFeatures()
            .AddUtils()
            .AddCharacterCreator();
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnStart()
    {
        ResourceExtensions.RegisterMValueAdapters();

        // TODO: Add logging for startup
        serviceProvider.GetServices<IStartup>();
        Alt.Log("loaded!!!");
    }

    public override void OnStop() { }

    public override IBaseObjectFactory<IWebView> GetWebViewFactory()
    {
        return serviceProvider.GetRequiredService<IUiViewFactory>();
    }
}
