using System.Text;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.SharedKernel;
using Proton.Shared.Dtos;

namespace Proton.Server.Resource.Features.Tuning.Scripts;
internal class TuningScript(IDbContextFactory dbFactory) : HostedService
{
    private readonly IDbContextFactory dbFactory = dbFactory;
    private readonly Dictionary<IPlayer, IVehicle> tuningStore = new Dictionary<IPlayer, IVehicle>();

    public override Task StartAsync(CancellationToken ct)
    {
        //just for development
        //Alt.OnPlayerConnect += Alt_OnPlayerConnect;
        Alt.OnClient("tune", OnTuningEnter);
        Alt.OnClient("tune:close", OnTuningLeave);
        Alt.OnClient<TuningDetailDto>("tune:preview:item", OnPreviewItem);
        Alt.OnClient("auth:complete", Alt_OnPlayerConnect);

        return Task.CompletedTask;
    }

    private void Alt_OnPlayerConnect(IPlayer player)
    {
        var position = new Position(486.417f, -3339.692f, 6.070f);
        player.Spawn(position);
        player.Model = (uint)PedModel.FreemodeMale01;

        var db = dbFactory.CreateDbContext();
        var _player = (PPlayer)player;
        var user = db.Users.Where(x => x.Id == _player.ProtonId).FirstOrDefault();
        var vehs = db.Vehicles.FirstOrDefault();
        if (user != null && user.OwnedVehicles.Count == 0 && vehs != null)
        {
            var _vehs = (OwnedVehicle)vehs;
            _vehs.PurchasedDate = DateTime.UtcNow;
            user.OwnedVehicles.Add(_vehs);

            db.Users.Update(user);
            db.SaveChangesAsync().GetAwaiter();
        }
    }

    private void OnTuningLeave(IPlayer player)
    {
        if(tuningStore.TryGetValue(player, out var value))
        {           
            applyModsAsync(value, new List<TuningDetailDto>()).GetAwaiter();

            tuningStore.Remove(player);
        }
    }

    private void OnTuningEnter(AltV.Net.Elements.Entities.IPlayer player)
    {
        if (!player.IsInVehicle){
            var db = dbFactory.CreateDbContext();
            var _player = (PPlayer)player;
            var user = db.Users.Where(x => x.Id == _player.ProtonId).FirstOrDefault();
            var ownedVeh = user?.OwnedVehicles.FirstOrDefault();
            if (user != null && ownedVeh != null)
            {
                var veh = Alt.CreateVehicle(ownedVeh.AltVHash, player.Position, player.Rotation);
                applyModsAsync(veh, ownedVeh.Tunings.Select(x=> new TuningDetailDto {CategoryId = x.CategoryId, Name = x.DisplayName, Value = Encoding.ASCII.GetBytes(x.Value)[0] }).ToList()).GetAwaiter();
            }
            //TODO: REMOVE, Just for dev
            return;
        }
        
        player.Emit("tuning:items:available", getTuningItems());
        tuningStore.Add(player, player.Vehicle); //TODO: Add vehicle player sits in
    }

    private TuningDto getTuningItems()
    {
        var tunings = new List<TuningWrapperDto>();
        var items = Enum.GetValues(typeof(VehicleModType));
        
        foreach(VehicleModType i in items)
        {
            if (ExcludeList.Excluded.Contains((int)i))
            {
                continue;
            }
            var t = getTuningById((int)i);

            tunings.Add(new TuningWrapperDto
            {
                Name = i.ToString(),
                Tunings = t
            });
        }
        return new()
        {
            TuningWrappers = tunings
        };
    }

    private List<TuningDetailDto> getTuningById(int Id)
    {
        var db = dbFactory.CreateDbContext();
        var res = db.VehicleTunings.Where(x => x.CategoryId == Id).ToList();
        return res.Select(x => new TuningDetailDto {
            Name = x.DisplayName,
            Price = x.Price,
            CategoryId = x.CategoryId,
            Value = Encoding.ASCII.GetBytes(x.Value)[0]
        }).ToList();
    }

    private async Task applyModsAsync(IVehicle vehicle, List<TuningDetailDto> mods)
    {
        foreach(var mod in mods)
        {
            switch (mod.CategoryId)
            {
                case (int)VehicleModType.Color1:
                    await vehicle.SetPrimaryColorAsync(mod.Value).ConfigureAwait(false);
                    break;
                case (int)VehicleModType.Color2:
                    await vehicle.SetSecondaryColorAsync(mod.Value).ConfigureAwait(false);
                    break;
                default:
                    vehicle.SetMod((byte)mod.CategoryId, (byte)mod.Value);
                    break;
            }
        }
    }

    private void OnPreviewItem(IPlayer player, TuningDetailDto item)
    {
        if (player.IsInVehicle)
        {
            applyModsAsync(player.Vehicle, new List<TuningDetailDto> { item }).GetAwaiter();
        }
    }
}   
