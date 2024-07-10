using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Core.Interfaces;
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
            .AddSingleton<NotificationService>(); //TODO: Add Interface
        return serviceCollection;
    }
}
