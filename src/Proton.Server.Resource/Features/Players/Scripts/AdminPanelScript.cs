using AltV.Net;
using AltV.Net.Enums;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;
using Proton.Server.Resource.Features.Vehicles.Abstractions;
using AltV.Net.Data;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Core.Models;
using Proton.Server.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using AsyncAwaitBestPractices;

namespace Proton.Server.Resource.Features.Players.Scripts;

public sealed class AdminPanelScript(IDbContextFactory dbFactory) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnClient<PPlayer>("admin-panel.mount", OnMount);
        Alt.OnClient<PPlayer>("admin-panel.players.get", OnPlayersGet);
        Alt.OnClient<PPlayer>("admin-panel.vehicles.get", OnVehiclesGet);
        Alt.OnClient<PPlayer, uint, string>("admin-panel.players.action", OnPlayersAction);
        Alt.OnClient<PPlayer, string>("admin-panel.vehicles.create", OnVehiclesCreate);
        Alt.OnClient<PPlayer, uint>("admin-panel.vehicles.destroy", OnVehiclesDestroy);
        return Task.CompletedTask;
    }

    private void OnMount(PPlayer player)
    {
        if (!IsModOrAdmin(player))
        {
            return;
        }

        player.Emit("admin-panel.mount", new AdminPanelMountDto
        {
            Tab = 0,
            Players = GetPlayerDtos()
        });
    }

    private static bool IsModOrAdmin(PPlayer player)
    {
        return player.Role == UserRole.Moderator || player.Role == UserRole.Administrator;
    }

    private static List<AdminPanelPlayerDto> GetPlayerDtos()
    {
        return Alt
            .GetAllPlayers()
            .Select(a => new AdminPanelPlayerDto
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToList();
    }

    private void OnPlayersGet(PPlayer player)
    {
        if (!IsModOrAdmin(player)) return;
        player.Emit("admin-panel.players.get", GetPlayerDtos());
    }

    private void OnVehiclesGet(PPlayer player)
    {
        if (!IsModOrAdmin(player)) return;
        var unknown = "Unknown";
        player.Emit("admin-panel.vehicles.get", Alt
            .GetAllVehicles()
            .Where(a => a is IProtonVehicle protonVehicle && protonVehicle.AdminPanelFlag)
            .Select(a => new AdminPanelVehicleDto
            {
                Id = a.Id,
                Name = Enum.GetName((VehicleModel)a.Model) ?? unknown,
            })
            .ToList());
    }

    private void OnPlayersAction(PPlayer player, uint id, string action)
    {
        var target = (PPlayer?)Alt.GetPlayerById(id);
        if (target is null)
        {
            return;
        }
        switch (action)
        {
            case "kick":
                if (!IsModOrAdmin(player)) break;
                target.Kick("Kicked by a moderator/admin");
                break;
            case "ban":
                if (player.Role != UserRole.Administrator) break;
                BanAsync(target).SafeFireAndForget(e => Alt.LogError(e.ToString()));
                break;
            case "tp":
                if (!IsModOrAdmin(player)) break;
                player.Position = target.Position;
                break;
            case "bring":
                if (!IsModOrAdmin(player)) break;
                target.Position = player.Position;
                break;
        }
    }

    private async Task BanAsync(PPlayer player)
    {
        await using var db = await dbFactory.CreateDbContextAsync().ConfigureAwait(false);
        var user = await db.Users
            .Where(a => a.Id == player.ProtonId)
            .Select(a => new { a.DiscordId })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        if (user is null)
        {
            return;
        }

        player.Kick("Banned by an admin");

        db.Add(new BanRecord
        {
            Kind = BanKind.Discord,
            Value = user.DiscordId.ToString()
        });
        await db.SaveChangesAsync().ConfigureAwait(false);
    }

    private void OnVehiclesCreate(PPlayer player, string name)
    {
        if (!IsModOrAdmin(player)) return;
        if (!Enum.TryParse<VehicleModel>(name, true, out var vehicleModel))
        {
            return;
        }
        var vehicle = (IProtonVehicle)Alt.CreateVehicle(vehicleModel, player.Position, Rotation.Zero);
        vehicle.AdminPanelFlag = true;
        player.Emit("admin-panel.vehicles.create", new AdminPanelVehicleDto
        {
            Id = vehicle.Id,
            Name = vehicleModel.ToString(),
        });
    }

    private void OnVehiclesDestroy(PPlayer player, uint id)
    {
        if (!IsModOrAdmin(player)) return;
        var vehicle = (IProtonVehicle?)Alt.GetVehicleById(id);
        if (vehicle is null || !vehicle.AdminPanelFlag)
        {
            return;
        }

        vehicle.Destroy();
        player.Emit("admin-panel.vehicles.destroy", id);
    }
}
