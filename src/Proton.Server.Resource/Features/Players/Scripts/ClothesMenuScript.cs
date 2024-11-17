using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Players.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Players.Scripts;

public sealed class ClothesMenuScript(IDbContextFactory dbFactory, IClosetService closetService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AltAsync.OnClient<IPlayer, long, string, Task>("clothes-menu.option.change", OnOptionChangeAsync);
        Alt.OnPlayerSpawn += (player) =>
        {
            foreach (var component in Enum.GetValues<PedComponent>())
            {
                if (component == PedComponent.Face)
                {
                    continue;
                }
                var (drawable, texture) = GetMaleDefaultClothes(component);
                player.SetClothes((byte)component, drawable, texture, 0);
            }
        };
        return Task.CompletedTask;
    }

    private Task OnOptionChangeAsync(IPlayer player, long closetId, string value)
    {
        if (player is not PPlayer pplayer)
        {
            return Task.CompletedTask;
        }

        switch (value)
        {
            case "equip":
                return EquipAsync(pplayer, closetId);
            case "unequip":
                Unequip(pplayer, closetId);
                break;
        }

        return Task.CompletedTask;
    }

    private async Task EquipAsync(PPlayer player, long closetId)
    {
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var closet = await db
            .Closets.Where(a => a.Id == closetId)
            .Select(a => new
            {
                a.Id,
                a.ClothItem.Component,
                a.ClothItem.Drawable,
                a.ClothItem.Texture,
                a.ClothItem.Palette,
                a.ClothItem.DlcName
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (closet is null)
        {
            return;
        }

        var component = (PedComponent)closet.Component;
        if (closetService.TryGetEquippedClothes(player, component, out var cloth))
        {
            if (cloth.Id == closet.Id)
            {
                return;
            }
            closetService.UnsetEquipped(player, component);
            player.Emit("clothes-menu.option.change", cloth.Id, "unequip");
        }

        player.SetDlcClothes(
            (byte)closet.Component,
            (ushort)closet.Drawable,
            (byte)closet.Texture,
            (byte)closet.Palette,
            string.IsNullOrEmpty(closet.DlcName) ? 0 : Alt.Hash(closet.DlcName)
        );
        closetService.SetEquipped(
            player,
            component,
            new ClosetClothes
            {
                Id = closet.Id,
                Drawable = (ushort)closet.Drawable,
                Texture = (byte)closet.Texture,
                Palette = (byte)closet.Palette,
                Dlc = string.IsNullOrEmpty(closet.DlcName) ? 0 : Alt.Hash(closet.DlcName)
            }
        );
        player.Emit("clothes-menu.option.change", closetId, "equip");
    }

    private void Unequip(PPlayer player, long closetId)
    {
        if (!closetService.TryGetAllEquippedComponents(player, out var components))
        {
            return;
        }

        foreach (var pair in components)
        {
            if (pair.Value.Id == closetId)
            {
                var (drawable, texture) = GetMaleDefaultClothes(pair.Key);
                player.SetClothes((byte)pair.Key, drawable, texture, 0);
                closetService.UnsetEquipped(player, pair.Key);
                player.Emit("clothes-menu.option.change", closetId, "unequip");
                break;
            }
        }
    }

    private (ushort Drawable, byte Texture) GetMaleDefaultClothes(PedComponent component)
    {
        return component switch
        {
            PedComponent.Mask => (0, 0),
            PedComponent.Hair => (0, 0),
            PedComponent.Torso => (15, 0),
            PedComponent.Leg => (61, 0),
            PedComponent.BagAndParachute => (0, 0),
            PedComponent.Shoes => (34, 0),
            PedComponent.Accessory => (0, 0),
            PedComponent.Undershirt => (15, 0),
            PedComponent.Kevlar => (0, 0),
            PedComponent.Decal => (0, 0),
            PedComponent.Top => (15, 0),
            _ => (0, 0),
        };
    }

    private (ushort Drawable, byte Texture) GetFemaleDefaultClothes(PedComponent component)
    {
        return component switch
        {
            PedComponent.Mask => (0, 0),
            PedComponent.Hair => (0, 0),
            PedComponent.Torso => (15, 0),
            PedComponent.Leg => (17, 0),
            PedComponent.BagAndParachute => (0, 0),
            PedComponent.Shoes => (35, 0),
            PedComponent.Accessory => (0, 0),
            PedComponent.Undershirt => (14, 0),
            PedComponent.Kevlar => (0, 0),
            PedComponent.Decal => (0, 0),
            PedComponent.Top => (18, 0),
            _ => (0, 0),
        };
    }
}
