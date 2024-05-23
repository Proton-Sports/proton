using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Authentication.Extentions;
using Proton.Client.Resource.CharacterCreator.Extensions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Client.Resource.Utils.Extentions;
using Proton.Shared.Extensions;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource;

public sealed class ClientResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;

    public ClientResource()
    {
        var serviceCollection = new ServiceCollection()
            .AddInfrastructure()
            .AddUiViews()
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
