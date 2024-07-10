using AltV.Net.Client;
using AltV.Net.Client.Async;
using AltV.Net.Client.Elements.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Infrastructure.Extensions;
using Proton.Client.Resource.Authentication.Extentions;
using Proton.Client.Resource.CharacterCreator.Extensions;
using Proton.Client.Resource.Commons.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Client.Resource.Hud.Extensions;
using Proton.Client.Resource.Nametags.Extensions;
using Proton.Client.Resource.Utils.Extentions;
using Proton.Shared.Extensions;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource;

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
            .AddNametags()
            .AddHud()
            .AddUtils()
            .AddCharacterCreator()
            .AddIplFeatures();
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public override void OnStart()
    {
        StartAsync().Wait();
    }

    public override void OnStop()
    {
        StopAsync().Wait();
    }

    public override IBaseObjectFactory<IWebView> GetWebViewFactory()
    {
        return serviceProvider.GetRequiredService<IUiViewFactory>();
    }

    private Task StartAsync()
    {
        ResourceExtensions.RegisterMValueAdapters();
        serviceProvider.GetServices<IStartup>();
        return Task.WhenAll(
            serviceProvider.GetServices<IHostedService>().Select(x => x.StartAsync(CancellationToken.None))
        );
    }

    private Task StopAsync()
    {
        return Task.WhenAll(
            serviceProvider.GetServices<IHostedService>().Select(x => x.StopAsync(CancellationToken.None))
        );
    }
}
