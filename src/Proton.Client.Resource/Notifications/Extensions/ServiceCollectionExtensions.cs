using Microsoft.Extensions.DependencyInjection;
using Proton.Client.Resource.Notifications.Abstractions;
using Proton.Client.Resource.Notifications.Scripts;

namespace Proton.Client.Resource.Notifications.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationFeatures(this IServiceCollection services)
    {
        services.AddSingleton<INotificationService, NotificationService>().AddHostedService<NotificationScript>();
        return services;
    }
}
