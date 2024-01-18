using System.Numerics;
using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Infrastructure.Interfaces;

namespace Proton.Server.Infrastructure.Services;

public class DefaultNoClip : INoClip
{
    private readonly HashSet<IPlayer> noClipPlayers = [];
    public DefaultNoClip()
    {
        Alt.OnClient<IPlayer, Vector3>("noclip:default:move", HandleMove);
    }

    public bool IsStarted(IPlayer player) => noClipPlayers.Contains(player);

    public void Start(IPlayer player)
    {
        noClipPlayers.Add(player);
        player.Emit("noclip:start");
    }

    public void Stop(IPlayer player)
    {
        noClipPlayers.Remove(player);
        player.Emit("noclip:stop");
    }

    private void HandleMove(IPlayer player, Vector3 position)
    {
        player.Position = position;
    }
}
