using AltV.Net.Client;
using Proton.Client.Resource.Notifications.Abstractions;

namespace Proton.Client.Resource.Notifications;

public class NotificationService : INotificationService
{
    //TODO: Load custom images https://natives.altv.mp/0xDFA2EF8E04127DD5
    public void DrawNotification(string image, string header, string details, string message)
    {
        Alt.Natives.BeginTextCommandThefeedPost("STRING");
        Alt.Natives.AddTextComponentSubstringPlayerName(message);
        Alt.Natives.EndTextCommandThefeedPostMessagetextTu(
            image.ToUpper(),
            image.ToUpper(),
            false,
            4,
            header,
            details,
            1.0f
        );
        Alt.Natives.EndTextCommandThefeedPostTicker(false, false);
    }
}
