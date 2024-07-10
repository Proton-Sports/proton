using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Client.Resource.Commons;

namespace Proton.Client.Resource.Features.NoClips.Scripts;

public sealed class NoClipScript(INoClip noClip) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer("noclip:start", HandleStart);
        Alt.OnServer("noclip:stop", HandleStop);
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken ct)
    {
        if (noClip.IsStarted)
        {
            noClip.Stop();
        }
        return Task.CompletedTask;
    }

    private void HandleStart()
    {
        noClip.Start();
    }

    private void HandleStop()
    {
        noClip.Stop();
    }
}
