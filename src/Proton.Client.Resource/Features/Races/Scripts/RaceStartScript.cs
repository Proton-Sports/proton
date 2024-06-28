using AltV.Net.Client;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceStartScript : IStartup
{
    private readonly IRaceService raceService;
    private readonly IUiView uiView;

    public RaceStartScript(IRaceService raceService, IUiView uiView)
    {
        this.raceService = raceService;
        this.uiView = uiView;
        Alt.OnServer<RaceStartDto>("race-start:start", HandleServerStart);
        Alt.OnConnectionComplete += HandleConnectionComplete;
    }

    private void HandleConnectionComplete()
    {
        Alt.Natives.SetLocalPlayerAsGhost(false, false);
    }

    private void HandleServerStart(RaceStartDto dto)
    {
        Alt.GameControlsEnabled = true;
        if (dto.Ghosting)
        {
            Alt.Natives.SetLocalPlayerAsGhost(true, true);
            foreach (var vehicle in Alt.GetAllVehicles())
            {
                if (vehicle != Alt.LocalPlayer.Vehicle && vehicle.Spawned)
                {
                    Alt.Natives.SetEntityGhostedForGhostPlayers(vehicle, true);
                }
            }
        }
        raceService.Start();
        uiView.Unmount(Route.RacePrepare);
    }
}
