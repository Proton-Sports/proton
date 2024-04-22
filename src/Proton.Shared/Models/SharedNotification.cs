using AltV.Community.MValueAdapters.Generators;
using AltV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Shared.Models
{
    [MValueAdapter]
    public class SharedNotification
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
}
