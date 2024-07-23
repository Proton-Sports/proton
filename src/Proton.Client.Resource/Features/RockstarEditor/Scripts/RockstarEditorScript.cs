using AltV.Net.Client;
using Proton.Client.Resource.Commons;

namespace Proton.Client.Resource.Features.RockstarEditor.Scripts;

public sealed class RockstarEditorScript : HostedService
{
    private uint interval;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnConsoleCommand += OnConsoleCommand;
        return Task.CompletedTask;
    }

    private void OnConsoleCommand(string command, string[] args)
    {
        switch (command)
        {
            case "start":
                Alt.Natives.StartReplayRecording(1);
                break;
            case "save":
                var result = Alt.Natives.SaveReplayRecording();
                Alt.Natives.StopReplayRecording();
                break;
            case "editor":
                Alt.Natives.ActivateRockstarEditor(1);
                interval = Alt.SetInterval(() =>
                {
                    if (Alt.Natives.IsScreenFadedOut())
                    {
                        Alt.Natives.DoScreenFadeIn(1000);
                        Alt.ClearInterval(interval);
                    }
                }, 1000);
                break;
        }
    }
}
