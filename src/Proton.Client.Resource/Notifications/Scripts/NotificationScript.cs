using AltV.Net.Client;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Notifications.Abstractions;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Notifications.Scripts;

internal class NotificationScript(INotificationService notificationService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<NotificationDto>("player:sendNotification", HandleServerNotification);
        return Task.CompletedTask;
    }

    private void HandleServerNotification(NotificationDto notify)
    {
        notificationService.DrawNotification(notify.Icon, notify.Title, notify.SecondaryTitle, notify.Body);
    }
}
