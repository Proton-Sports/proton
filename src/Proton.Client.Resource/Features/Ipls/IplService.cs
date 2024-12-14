using AltV.Net.Client;
using AltV.Net.Client.Async;
using Proton.Client.Resource.Features.Ipls.Abstractions;

namespace Proton.Client.Resource.Features.Ipls;

public sealed class IplService : IIplService
{
    public IplService()
    {
        Alt.OnServer<string>("ipl:load", HandleLoadIpl);
        Alt.OnServer<long, string, Task>("ipl:loadAsync", HandleLoadIplAsync);
        Alt.OnServer<string>("ipl:unload", HandleUnloadIpl);
        Alt.OnConsoleCommand += (_, _2) =>
        {
            Alt.LogInfo("sport_zancudo_01: " + Alt.Natives.IsIplActive("sport_zancudo_01"));
            Alt.LogInfo(
                "sport_vinewood_hills_racetrack_01: " + Alt.Natives.IsIplActive("sport_vinewood_hills_racetrack_01")
            );
            Alt.LogInfo("sport_tongva_road_course_01: " + Alt.Natives.IsIplActive("sport_tongva_road_course_01"));
            Alt.LogInfo("sport_senora_raceway_01: " + Alt.Natives.IsIplActive("sport_senora_raceway_01"));
            Alt.LogInfo("sport_santa_monica_pier_gp_01: " + Alt.Natives.IsIplActive("sport_santa_monica_pier_gp_01"));
            Alt.LogInfo("sport_rheinland_grand_prix_01: " + Alt.Natives.IsIplActive("sport_rheinland_grand_prix_01"));
            Alt.LogInfo("sport_redwood_light_raceway_01: " + Alt.Natives.IsIplActive("sport_redwood_light_raceway_01"));
            Alt.LogInfo("sport_paleto_bay_gp_01: " + Alt.Natives.IsIplActive("sport_paleto_bay_gp_01"));
            Alt.LogInfo("sport_harbor_500_01: " + Alt.Natives.IsIplActive("sport_harbor_500_01"));
            Alt.LogInfo(
                "sport_eastlossantosrallyss_02_01: " + Alt.Natives.IsIplActive("sport_eastlossantosrallyss_02_01")
            );
            Alt.LogInfo(
                "sport_east_los_santos_rally_ss2_01: " + Alt.Natives.IsIplActive("sport_east_los_santos_rally_ss2_01")
            );
            Alt.LogInfo("sport_del_perro_racetrack_01: " + Alt.Natives.IsIplActive("sport_del_perro_racetrack_01"));
            Alt.LogInfo("sport_bolingbroke_rx_01: " + Alt.Natives.IsIplActive("sport_bolingbroke_rx_01"));
            Alt.LogInfo("sport_atomic_downtown_gp_01: " + Alt.Natives.IsIplActive("sport_atomic_downtown_gp_01"));
        };
    }

    public bool IsLoaded(string name)
    {
        return Alt.Natives.IsIplActive(name);
    }

    public async Task<bool> LoadAsync(string name)
    {
        Alt.RequestIpl(name);
        try
        {
            await AltAsync.WaitFor(() => Alt.Natives.IsIplActive(name), timeout: 3000, interval: 30);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task<bool> UnloadAsync(string name)
    {
        Alt.RemoveIpl(name);
        try
        {
            await AltAsync.WaitFor(() => !Alt.Natives.IsIplActive(name), timeout: 3000, interval: 30);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    private void HandleLoadIpl(string name)
    {
        Alt.RequestIpl(name);
    }

    private void HandleUnloadIpl(string name)
    {
        Alt.RemoveIpl(name);
    }

    private async Task HandleLoadIplAsync(long id, string name)
    {
        await LoadAsync(name).ConfigureAwait(false);
        Alt.EmitServer("ipl:loadAsync", id);
    }
}
