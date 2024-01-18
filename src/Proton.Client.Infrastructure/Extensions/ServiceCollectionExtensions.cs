using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Client.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IRaceCreator, LandRaceCreator>();
        serviceCollection.AddSingleton<IGameplayCamera, DefaultGameplayCamera>();
        serviceCollection.AddSingleton<IScriptCameraFactory, DefaultScriptCameraFactory>();
        serviceCollection.AddSingleton<IRaycastService, DefaultRaycastService>();
        serviceCollection.AddSingleton<INoClip, DefaultNoClip>();
        serviceCollection.AddSingleton<IUiViewFactory, DefaultUiViewFactory>();
        serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://localhost:5173"));
        // serviceCollection.AddSingleton(provider => (IUiView)Alt.CreateWebView("http://assets/proton-ui/dist/index.html"));
        return serviceCollection;
    }
}
