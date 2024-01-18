using AltV.Net.Elements.Entities;

namespace Proton.Server.Infrastructure.Interfaces;

public interface INoClip
{
    bool IsStarted(IPlayer player);
    void Start(IPlayer player);
    void Stop(IPlayer player);
}
