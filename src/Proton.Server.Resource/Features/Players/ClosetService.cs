using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Players.Abstractions;

namespace Proton.Server.Resource.Features.Players;

public sealed class ClosestService : IClosetService
{
    private readonly ConcurrentDictionary<IPlayer, ConcurrentDictionary<PedComponent, ClosetClothes>> playerComponents =
        [];

    public bool TryGetAllEquippedComponents(
        IPlayer player,
        [NotNullWhen(true)] out IReadOnlyDictionary<PedComponent, ClosetClothes>? clothes
    )
    {
        if (playerComponents.TryGetValue(player, out var components))
        {
            clothes = components;
            return true;
        }

        clothes = default;
        return false;
    }

    public bool TryGetEquippedClothes(IPlayer player, PedComponent component, out ClosetClothes clothes)
    {
        if (playerComponents.TryGetValue(player, out var components) && components.TryGetValue(component, out clothes))
        {
            return true;
        }

        clothes = default;
        return false;
    }

    public void SetEquipped(IPlayer player, PedComponent component, ClosetClothes clothes)
    {
        if (!playerComponents.TryGetValue(player, out var components))
        {
            components = new() { [component] = clothes };
            playerComponents[player] = components;
            return;
        }

        components[component] = clothes;
    }

    public bool UnsetEquipped(IPlayer player, PedComponent component)
    {
        if (!playerComponents.TryGetValue(player, out var components) || !components.Remove(component, out _))
        {
            return false;
        }

        if (components.Keys.Count == 0)
        {
            playerComponents.Remove(player, out _);
        }
        return true;
    }

    public bool UnsetEquipped(IPlayer player)
    {
        return playerComponents.Remove(player, out _);
    }
}
