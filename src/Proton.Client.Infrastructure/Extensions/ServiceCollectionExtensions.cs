using AltV.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Infrastructure.Services;

namespace Proton.Client.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IGameplayCamera, DefaultGameplayCamera>()
            .AddSingleton<IScriptCameraFactory, DefaultScriptCameraFactory>()
            .AddSingleton<IRaycastService, DefaultRaycastService>()
            .AddSingleton<INoClip, ClientOnlyNoClip>()
            .AddSingleton<IUiViewFactory, DefaultUiViewFactory>();
        serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://localhost:5173"));
        //serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://assets/proton-ui/dist/index.html"));
        return serviceCollection;
    }
}
