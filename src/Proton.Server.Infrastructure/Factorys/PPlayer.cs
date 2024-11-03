using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using Proton.Shared.Dtos;

namespace Proton.Server.Infrastructure.Factorys;

public class PPlayer(ICore core, nint nativePointer, uint id) : AsyncPlayer(core, nativePointer, id)
{
    //If -1 Player not Loggedin
    public long ProtonId { get; set; } = 1;

    public void SendNotification(NotificationDto notify) => Player.Emit("player:sendNotification", notify);

    public void ClearClothing()
    {
        SetClothes(3, 15, 0, 0); // Torso
        SetClothes(11, 15, 0, 0); // Shirt
        SetClothes(8, 15, 0, 0); // Chest Part Of The Shirt
    }
}
