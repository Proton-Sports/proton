using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Ipls.Abstractions;

public interface IIplService
{
    bool Load(IPlayer player, string name);
    Task<bool> LoadAsync(IEnumerable<IPlayer> players, string name);
    bool Unload(IPlayer player, string name);
}
