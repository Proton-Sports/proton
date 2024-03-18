using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IRaceCreator, LandRaceCreator>()
            .AddSingleton<IGameplayCamera, DefaultGameplayCamera>()
            .AddSingleton<IScriptCameraFactory, DefaultScriptCameraFactory>()
            .AddSingleton<IRaycastService, DefaultRaycastService>()
            // .AddSingleton<INoClip, DefaultNoClip>()
            .AddSingleton<INoClip, ClientOnlyNoClip>()
            .AddSingleton<IUiViewFactory, DefaultUiViewFactory>();
        // serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://localhost:5173"));
        serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://assets/proton-ui/dist/index.html"));
        return serviceCollection;
    }
}
