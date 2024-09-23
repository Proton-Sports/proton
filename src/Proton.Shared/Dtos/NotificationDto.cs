using AltV.Community.MValueAdapters.Generators;

namespace Proton.Shared.Dtos;

[MValueAdapter]
public class NotificationDto
{
    public string Title { get; set; } = "";
    public string SecondaryTitle { get; set; } = "";
    public string Body { get; set; } = "";
    public string Icon { get; set; } = NotificationIcons.DEFAULT;
}

// List of all Icons
// https://wiki.rage.mp/index.php?title=Notification_Pictures
public static class NotificationIcons
{
    public const string DEFAULT = "CHAR_DEFAULT";
    public const string CARSITE = "CHAR_CARSITE";
    public const string LS_CUSTOMS = "CHAR_LS_CUSTOMS";
}
