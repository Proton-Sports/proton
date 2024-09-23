using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using Proton.Shared.Dtos;

namespace Proton.Server.Infrastructure.Factorys;

public class PPlayer(ICore core, nint nativePointer, uint id) : AsyncPlayer(core, nativePointer, id)
{
    //If -1 Player not Loggedin
    public long ProtonId { get; set; } = -1;

    public void SendNotification(NotificationDto notify) => Player.Emit("player:sendNotification", notify);
}
