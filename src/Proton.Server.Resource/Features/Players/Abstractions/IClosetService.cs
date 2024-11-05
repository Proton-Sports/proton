using System.Diagnostics.CodeAnalysis;
using AltV.Net.Elements.Entities;

namespace Proton.Server.Resource.Features.Players.Abstractions;

public interface IClosetService
{
    bool TryGetAllEquippedComponents(
        IPlayer player,
        [NotNullWhen(true)] out IReadOnlyDictionary<PedComponent, ClosetClothes>? clothes
    );
    bool TryGetEquippedClothes(IPlayer player, PedComponent component, out ClosetClothes clothes);
    void SetEquipped(IPlayer player, PedComponent component, ClosetClothes clothes);
    bool UnsetEquipped(IPlayer player);
    bool UnsetEquipped(IPlayer player, PedComponent component);
}
