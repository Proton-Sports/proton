using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;

namespace Proton.Client.Resource.Features.RockstarEditor.Scripts;

public sealed class RockstarEditorScript : HostedService
{
    private uint interval;
    private bool recording;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnKeyUp += OnKeyUp;
        return Task.CompletedTask;
    }

    private void OnKeyUp(Key key)
    {
        switch (key)
        {
            case Key.F9:
                if (recording)
                {
                    var result = Alt.Natives.SaveReplayRecording();
                    Alt.Natives.StopReplayRecording();
                    Alt.Log($"Save replay recording result: {result}.");
                }
                else
                {
                    Alt.Natives.StartReplayRecording(1);
                }
                recording = !recording;
                break;
            case Key.F10:
                Alt.Natives.ActivateRockstarEditor(1);
                interval = Alt.SetInterval(
                    () =>
                    {
                        if (Alt.Natives.IsScreenFadedOut())
                        {
                            Alt.Natives.DoScreenFadeIn(1000);
                            Alt.ClearInterval(interval);
                        }
                    },
                    1000
                );
                break;
        }
    }
}
