using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceRespawnScript(IRaceService raceService) : HostedService
{
    private uint holdingInterval;
    private DateTimeOffset holdingStartTime;

    public override Task StartAsync(CancellationToken ct)
    {
        raceService.Started += OnStarted;
        raceService.Stopped += OnStopped;
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        Alt.OnKeyDown += OnKeyDown;
        Alt.OnKeyUp += OnKeyUp;
        Alt.OnTick += OnTick;
    }

    private void OnTick()
    {
        Alt.Natives.DisableControlAction(0, 80, true);
    }

    private void OnStopped()
    {
        Alt.OnKeyDown -= OnKeyDown;
        Alt.OnKeyUp -= OnKeyUp;
        Alt.OnTick -= OnTick;
    }

    private void OnKeyDown(Key key)
    {
        if (key == Key.R)
        {
            holdingStartTime = DateTimeOffset.UtcNow;
            holdingInterval = Alt.SetInterval(OnHoldingR, 50);
        }
    }

    private void OnKeyUp(Key key)
    {
        if (key == Key.R && holdingStartTime != DateTimeOffset.MinValue)
        {
            StopHoldingDetect();
        }
    }

    private void OnHoldingR()
    {
        if ((DateTimeOffset.UtcNow - holdingStartTime).TotalSeconds >= 1)
        {
            Alt.EmitServer("race-respawn:respawn");
            StopHoldingDetect();
        }
    }

    private void StopHoldingDetect()
    {
        holdingStartTime = DateTimeOffset.MinValue;
        Alt.ClearInterval(holdingInterval);
    }
}
