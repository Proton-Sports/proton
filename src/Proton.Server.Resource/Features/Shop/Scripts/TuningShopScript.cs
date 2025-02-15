using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Core.Models.Shop;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Features.Vehicles;
using Proton.Server.Resource.Features.Vehicles.Abstractions;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Shop.Scripts;

public sealed class TuningShopScript(IDbContextFactory dbFactory) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnClient<PPlayer>("tuning-shop.mount", OnMount);
        Alt.OnClient<PPlayer, int, int>("tuning-shop.values.change", OnValuesChange);
        Alt.OnClient<PPlayer, int, int>("tuning-shop.wheels.change", OnWheelsChange);
        AltAsync.OnClient<PPlayer, long>(
            "tuning-shop.dev.spawn",
            async (player, id) =>
            {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
                var playerVehicle = await db
                    .PlayerVehicles.Where(a => a.Id == id)
                    .Select(a => new
                    {
                        a.Id,
                        a.Model,
                        a.PrimaryColor,
                        a.SecondaryColor,
                        Mods = a
                            .Mods.Where(b => b.PlayerVehicleActiveMod != null)
                            .Select(a => new { a.Mod.Category, a.Mod.Value }),
                        WheelVariations = a
                            .WheelVariations.Where(b => b.PlayerVehicleActiveWheelVariation != null)
                            .Select(a => new { a.WheelVariation.Type, a.WheelVariation.Value })
                    })
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (playerVehicle is null)
                {
                    return;
                }
                var vehicle = (IProtonVehicle)(
                    await AltAsync
                        .CreateVehicle(playerVehicle.Model, player.Position, player.Rotation)
                        .ConfigureAwait(false)
                );
                vehicle.GarageId = playerVehicle.Id;
                vehicle.Dimension = player.Dimension;
                vehicle.ModKit = 1;
                var wheel = playerVehicle.WheelVariations.FirstOrDefault();
                if (wheel is not null)
                {
                    vehicle.SetWheels((byte)wheel.Type, (byte)wheel.Value);
                }
                foreach (var mod in playerVehicle.Mods)
                {
                    vehicle.SetMod((byte)mod.Category, (byte)mod.Value);
                }
                vehicle.PrimaryColorRgb = playerVehicle.PrimaryColor;
                vehicle.SecondaryColorRgb = playerVehicle.SecondaryColor;
                player.SetIntoVehicle(vehicle, 1);
            }
        );
        AltAsync.OnClient<PPlayer, TuningShopGenerateDto, Task>(
            "tuning-shop.dev.generate",
            async (player, dto) =>
            {
                Alt.LogInfo("tuning-shop.dev.generate start");
                var vehicle = player.Vehicle;
                if (vehicle is null)
                {
                    return;
                }

                Alt.LogInfo("tuning-shop.dev.generate vehicle");
                await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);

                foreach (var wheel in dto.Wheels)
                {
                    for (var i = 0; i != wheel.Count; ++i)
                    {
                        db.Add(
                            new WheelVariation
                            {
                                Model = (VehicleModel)vehicle.Model,
                                Type = wheel.Type,
                                Name = $"{wheel.Type} {i + 1}",
                                Value = i,
                                Price = 50,
                            }
                        );
                    }
                }
                foreach (
                    var category in Enum.GetValues<VehicleModType>()
                        .Where(a => a != VehicleModType.FrontWheels && a != VehicleModType.BackWheels)
                )
                {
                    var count = vehicle.GetModsCount((byte)category);
                    for (var i = 0; i != count; ++i)
                    {
                        db.Add(
                            new Mod
                            {
                                Category = (int)category,
                                Model = (VehicleModel)vehicle.Model,
                                Price = 50,
                                Name = $"{category} {i + 1}",
                                Value = i + 1
                            }
                        );
                    }
                }

                try
                {
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Alt.LogError(e.ToString());
                    throw;
                }
                Alt.LogInfo("tuning-shop.dev.generate end");
            }
        );
        Alt.OnClient<PPlayer, int, int>(
            "tuning-shop.dev.wheel",
            (player, a, b) =>
            {
                player.Vehicle?.SetWheels((byte)a, (byte)b);
            }
        );
        AltAsync.OnClient<PPlayer, int, long, Task>("tuning-shop.buy", OnBuyAsync);
        AltAsync.OnClient<PPlayer, int, string, Task>("tuning-shop.colors.buy", OnBuyColorAsync);
        AltAsync.OnClient<PPlayer, long, Task>("tuning-shop.wheels.buy", OnBuyWheelsAsync);
        AltAsync.OnClient<PPlayer, int, long, bool, Task>("tuning-shop.toggle", OnToggleAsync);
        AltAsync.OnClient<PPlayer, TuningShopWheelVariationDto, bool, Task>(
            "tuning-shop.wheels.toggle",
            OnWheelsToggleAsync
        );
        AltAsync.OnClient<PPlayer, Task>("tuning-shop.unmount", OnUnmountAsync);
        Alt.OnClient<PPlayer, int, string>(
            "tuning-shop.colors.change",
            (player, category, hex) =>
            {
                var vehicle = player.Vehicle;
                if (vehicle is null)
                {
                    return;
                }
#pragma warning disable CA1846 // Prefer 'AsSpan' over 'Substring'
                if (
                    !byte.TryParse(hex.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)
                    || !byte.TryParse(
                        hex.Substring(3, 2),
                        NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture,
                        out var g
                    )
                    || !byte.TryParse(
                        hex.Substring(5, 2),
                        NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture,
                        out var b
                    )
                )
                {
                    return;
                }
#pragma warning restore CA1846 // Prefer 'AsSpan' over 'Substring'

                var color = new Rgba(r, g, b, 255);
                switch (category)
                {
                    case 66: // primary
                        vehicle.PrimaryColorRgb = color;
                        break;
                    case 67: // secondary
                        vehicle.SecondaryColorRgb = color;
                        break;
                    default:
                        break;
                }
            }
        );
        AltAsync.OnClient<PPlayer, int, Task>("tuning-shop.mods.requestData", OnModsRequestDataAsync);
        AltAsync.OnClient<PPlayer, Task>("tuning-shop.wheels.requestData", OnWheelsRequestDataAsync);
        return Task.CompletedTask;
    }

    void OnMount(PPlayer player)
    {
        if (player.Vehicle is not IProtonVehicle vehicle)
        {
            return;
        }

        player.Emit(
            "tuning-shop.mount",
            new TuningShopMountDto
            {
                PrimaryColor = vehicle.PrimaryColorRgb,
                SecondaryColor = vehicle.SecondaryColorRgb,
            }
        );
    }

    void OnValuesChange(PPlayer player, int category, int value)
    {
        var vehicle = player.Vehicle;
        if (vehicle is null)
        {
            return;
        }

        vehicle.SetMod((byte)category, (byte)value);
    }

    void OnWheelsChange(PPlayer player, int type, int value)
    {
        var vehicle = player.Vehicle;
        if (vehicle is null)
        {
            return;
        }

        vehicle.SetWheels((byte)type, (byte)value);
    }

    async Task OnBuyAsync(PPlayer player, int category, long modId)
    {
        if (player.Vehicle is not IProtonVehicle vehicle)
        {
            return;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        await using var transaction = await db
            .Database.BeginTransactionAsync(IsolationLevel.RepeatableRead)
            .ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        var mod = await db
            .Mods.Where(a => a.Id == modId)
            .Select(a => new { a.Price })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (mod is null)
        {
            player.Emit("tuning-shop.buy", category, modId, false);
            return;
        }

        var user = await db
            .Users.Where(a => a.Id == player.ProtonId)
            .Select(a => new { a.Money })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (user is null || user.Money < mod.Price)
        {
            player.Emit("tuning-shop.buy", category, modId, false);
            return;
        }

        db.Add(
            new PlayerVehicleMod
            {
                ModId = modId,
                PlayerVehicleId = vehicle.GarageId,
                PlayerVehicleActiveMod = new PlayerVehicleActiveMod { }
            }
        );
        try
        {
            await db
                .PlayerVehicleActiveMods.Where(a =>
                    a.PlayerVehicleMod.PlayerVehicleId == vehicle.GarageId
                    && a.PlayerVehicleMod.Mod.Category == category
                )
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
            await db
                .Users.Where(a => a.Id == player.ProtonId)
                .ExecuteUpdateAsync(a => a.SetProperty(b => b.Money, b => b.Money - mod.Price))
                .ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Alt.LogError(e.ToString());
            player.Emit("tuning-shop.buy", category, modId, false);
        }

        player.Emit("tuning-shop.buy", category, modId, true);
    }

    async Task OnBuyColorAsync(PPlayer player, int category, string hex)
    {
        if (
            player.Vehicle is not IProtonVehicle vehicle
            || (category != 66 && category != 67)
            || !byte.TryParse(hex.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)
            || !byte.TryParse(hex.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g)
            || !byte.TryParse(hex.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b)
        )
        {
            return;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        await using var transaction = await db
            .Database.BeginTransactionAsync(IsolationLevel.RepeatableRead)
            .ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        const int price = 50;
        var user = await db
            .Users.Where(a => a.Id == player.ProtonId)
            .Select(a => new { a.Money })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (user is null || user.Money < price)
        {
            player.Emit("tuning-shop.colors.buy", category, hex, false);
            return;
        }

        var color = new Rgba(r, g, b, 255);
        Expression<Func<SetPropertyCalls<PlayerVehicle>, SetPropertyCalls<PlayerVehicle>>> expression =
            category == 66
                ? a =>
                    a.SetProperty(b => b.PrimaryColor.R, color.R)
                        .SetProperty(b => b.PrimaryColor.G, color.G)
                        .SetProperty(b => b.PrimaryColor.B, color.B)
                : a =>
                    a.SetProperty(b => b.SecondaryColor.R, color.R)
                        .SetProperty(b => b.SecondaryColor.G, color.G)
                        .SetProperty(b => b.SecondaryColor.B, color.B);
        try
        {
            var count = await db
                .PlayerVehicles.Where(a => a.Id == vehicle.GarageId)
                .ExecuteUpdateAsync(expression)
                .ConfigureAwait(false);
            await db
                .Users.Where(a => a.Id == player.ProtonId)
                .ExecuteUpdateAsync(a => a.SetProperty(b => b.Money, b => b.Money - price))
                .ConfigureAwait(false);
            if (count > 0)
            {
                if (category == 66)
                {
                    vehicle.PrimaryColorRgb = color;
                }
                else
                {
                    vehicle.SecondaryColorRgb = color;
                }
            }
            await transaction.CommitAsync().ConfigureAwait(false);
            player.Emit("tuning-shop.colors.buy", category, hex, count > 0);
        }
        catch
        {
            player.Emit("tuning-shop.colors.buy", category, hex, false);
        }
    }

    async Task OnToggleAsync(PPlayer player, int category, long modId, bool value)
    {
        if (player.Vehicle is not IProtonVehicle vehicle)
        {
            return;
        }

        Console.WriteLine("OnToggleAsync: " + category + ", " + modId + ", " + value);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (value)
        {
            var playerVehicleMod = await db
                .PlayerVehicleMods.Where(a => a.PlayerVehicleId == vehicle.GarageId && a.ModId == modId)
                .Select(a => new { a.Id, a.Mod.Value })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (playerVehicleMod is null)
            {
                return;
            }
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var transaction = await db
                .Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)
                .ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            db.Add(new PlayerVehicleActiveMod { PlayerVehicleModId = playerVehicleMod.Id });
            try
            {
                await db
                    .PlayerVehicleActiveMods.Where(a =>
                        a.PlayerVehicleMod.PlayerVehicleId == vehicle.GarageId
                        && a.PlayerVehicleMod.Mod.Category == category
                    )
                    .ExecuteDeleteAsync()
                    .ConfigureAwait(false);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
                vehicle.SetMod((byte)category, (byte)playerVehicleMod.Value);
            }
            catch (Exception e)
            {
                Alt.LogError(e.ToString());
            }
        }
        else
        {
            await db
                .PlayerVehicleActiveMods.Where(a =>
                    a.PlayerVehicleMod.PlayerVehicleId == vehicle.GarageId
                    && a.PlayerVehicleMod.Mod.Category == category
                )
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
            vehicle.SetMod((byte)category, 0);
        }
    }

    async Task OnUnmountAsync(PPlayer player)
    {
        if (player.Vehicle is not IProtonVehicle vehicle)
        {
            return;
        }
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        var playerVehicle = await db
            .PlayerVehicles.Where(a => a.Id == vehicle.GarageId)
            .Select(a => new
            {
                a.PrimaryColor,
                a.SecondaryColor,
                Mods = a
                    .Mods.Where(b => b.PlayerVehicleActiveMod != null)
                    .Select(a => new { a.Mod.Category, a.Mod.Value }),
                WheelVariations = a
                    .WheelVariations.Where(b => b.PlayerVehicleActiveWheelVariation != null)
                    .Select(b => new { b.WheelVariation.Type, b.WheelVariation.Value })
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (playerVehicle is null)
        {
            return;
        }

        var modsDictionary = playerVehicle.Mods.ToDictionary(a => a.Category);
        foreach (var value in Enum.GetValues<VehicleModType>())
        {
            var category = (byte)value;
            if (modsDictionary.TryGetValue((int)value, out var mod))
            {
                if (vehicle.GetMod(category) != (byte)mod.Value)
                {
                    vehicle.SetMod(category, (byte)mod.Value);
                }
            }
            else
            {
                vehicle.SetMod(category, 0);
            }
        }
        var wheel = playerVehicle.WheelVariations.FirstOrDefault();
        if (wheel is null)
        {
            vehicle.SetWheels(0, 0);
        }
        else
        {
            vehicle.SetWheels((byte)wheel.Type, (byte)wheel.Value);
        }

        vehicle.PrimaryColorRgb = playerVehicle.PrimaryColor;
        vehicle.SecondaryColorRgb = playerVehicle.SecondaryColor;
    }

    async Task OnBuyWheelsAsync(PPlayer player, long wheelVariationId)
    {
        if (player.Vehicle is not ProtonVehicle vehicle)
        {
            return;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        await using var transaction = await db
            .Database.BeginTransactionAsync(IsolationLevel.RepeatableRead)
            .ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        var wheelVariation = await db
            .WheelVariations.Where(a => a.Id == wheelVariationId)
            .Select(a => new { a.Price })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        var user = await db
            .Users.Where(a => a.Id == player.ProtonId)
            .Select(a => new { a.Money })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (wheelVariation is null || user is null || user.Money < wheelVariation.Price)
        {
            player.Emit("tuning-shop.wheels.buy", wheelVariationId, false);
            await transaction.CommitAsync().ConfigureAwait(false);
            return;
        }

        db.Add(
            new PlayerVehicleWheelVariation
            {
                WheelVariationId = wheelVariationId,
                PlayerVehicleId = vehicle.GarageId,
                PlayerVehicleActiveWheelVariation = new PlayerVehicleActiveWheelVariation { }
            }
        );
        try
        {
            await db
                .PlayerVehicleActiveWheelVariations.Where(a =>
                    a.PlayerVehicleWheelVariation.PlayerVehicleId == vehicle.GarageId
                )
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
            await db
                .Users.Where(a => a.Id == player.ProtonId)
                .ExecuteUpdateAsync(a => a.SetProperty(b => b.Money, b => b.Money - wheelVariation.Price))
                .ConfigureAwait(false);
            await db.SaveChangesAsync().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Alt.LogError(e.ToString());
            player.Emit("tuning-shop.wheels.buy", wheelVariationId, false);
        }

        player.Emit("tuning-shop.wheels.buy", wheelVariationId, true);
    }

    async Task OnWheelsToggleAsync(PPlayer player, TuningShopWheelVariationDto dto, bool value)
    {
        if (player.Vehicle is not IProtonVehicle vehicle)
        {
            return;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (value)
        {
            var playerVehicleWheelVariation = await db
                .PlayerVehicleWheelVariations.Where(a =>
                    a.PlayerVehicleId == vehicle.GarageId && a.WheelVariationId == dto.Id
                )
                .Select(a => new { a.Id })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (playerVehicleWheelVariation is null)
            {
                return;
            }
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var transaction = await db
                .Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)
                .ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            db.Add(
                new PlayerVehicleActiveWheelVariation { PlayerVehicleWheelVariationId = playerVehicleWheelVariation.Id }
            );
            try
            {
                await db
                    .PlayerVehicleActiveWheelVariations.Where(a =>
                        a.PlayerVehicleWheelVariation.PlayerVehicleId == vehicle.GarageId
                    )
                    .ExecuteDeleteAsync()
                    .ConfigureAwait(false);
                await db.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);
                vehicle.SetWheels((byte)dto.Type, (byte)dto.Value);
            }
            catch (Exception e)
            {
                Alt.LogError(e.ToString());
            }
        }
        else
        {
            await db
                .PlayerVehicleActiveWheelVariations.Where(a =>
                    a.PlayerVehicleWheelVariation.PlayerVehicleId == vehicle.GarageId
                )
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
            vehicle.SetWheels(0, 0);
        }
    }

    async Task OnModsRequestDataAsync(PPlayer player, int category)
    {
        if (player.Vehicle is not ProtonVehicle vehicle)
        {
            return;
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        var mods = await db
            .Mods.Where(a => a.Category == category && (a.Model == null || a.Model == (VehicleModel)vehicle.Model))
            .ToListAsync()
            .ConfigureAwait(false);
        var playerVehicle = await db
            .PlayerVehicles.Where(a => a.Id == vehicle.GarageId)
            .Select(a => new
            {
                Mods = a.Mods.Select(a => new
                {
                    a.Mod.Category,
                    a.ModId,
                    IsActive = a.PlayerVehicleActiveMod != null
                }),
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        player.Emit(
            "tuning-shop.mods.requestData",
            category,
            new TuningShopRequestModDataDto
            {
                Mods =
                [
                    .. mods.Select(a => new TuningShopModDto
                    {
                        Id = a.Id,
                        Category = a.Category,
                        Model = (uint?)a.Model,
                        Price = a.Price,
                        Value = a.Value,
                        Name = a.Name
                    })
                ],
                OwnedMods =
                [
                    .. (
                        playerVehicle?.Mods.Select(a => new TuningShopOwnedModDto
                        {
                            ModId = a.ModId,
                            Category = a.Category,
                            IsActive = a.IsActive
                        }) ?? []
                    )
                ],
            }
        );
    }

    async Task OnWheelsRequestDataAsync(PPlayer player)
    {
        if (player.Vehicle is not IProtonVehicle vehicle)
        {
            return;
        }
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
        var mods = await db
            .Mods.Where(a => a.Model == null || a.Model == (VehicleModel)vehicle.Model)
            .ToListAsync()
            .ConfigureAwait(false);
        var wheelVariations = await db
            .WheelVariations.Where(a => a.Model == null || a.Model == (VehicleModel)vehicle.Model)
            .ToListAsync()
            .ConfigureAwait(false);
        var playerVehicle = await db
            .PlayerVehicles.Where(a => a.Id == vehicle.GarageId)
            .Select(a => new
            {
                WheelVariations = a.WheelVariations.Select(a => new
                {
                    a.WheelVariationId,
                    a.WheelVariation.Type,
                    a.WheelVariation.Value,
                    a.WheelVariation.Name,
                    a.WheelVariation.Price,
                    IsActive = a.PlayerVehicleActiveWheelVariation != null
                })
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        player.Emit(
            "tuning-shop.wheels.requestData",
            new TuningShopRequestWheelsDataDto
            {
                WheelVariations =
                [
                    .. wheelVariations.Select(a => new TuningShopWheelVariationDto
                    {
                        Id = a.Id,
                        Type = (int)a.Type,
                        Model = (uint?)a.Model,
                        Price = a.Price,
                        Value = a.Value,
                        Name = a.Name
                    })
                ],
                OwnedWheelVariations =
                [
                    .. (
                        playerVehicle?.WheelVariations.Select(a => new TuningShopOwnedWheelVariationDto
                        {
                            Type = (int)a.Type,
                            Value = a.Value,
                            WheelVariationId = a.WheelVariationId,
                            IsActive = a.IsActive
                        }) ?? []
                    )
                ],
            }
        );
    }
}
