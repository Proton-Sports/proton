namespace Proton.Client.Resource.Notifications.Abstractions;

public interface INotificationService
{
    public void DrawNotification(string image, string header, string details, string message);
}
