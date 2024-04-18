using AltV.Net.Client;
using Proton.Client.Infrastructure.Services;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Utils.Scripts
{
    internal class NotificationScript : IStartup
    {
        private readonly NotificationService notificationService;

        public NotificationScript(NotificationService notificationService)
        {
            this.notificationService = notificationService;
            Alt.OnServer<SharedNotification>("player:sendNotification", HandleServerNotification);
        }

        private void HandleServerNotification(SharedNotification notify)
        {
            notificationService.DrawNotification(notify.Icon,
                notify.Title,
                notify.SecondaryTitle,
                notify.Body);
        }
    }
}
