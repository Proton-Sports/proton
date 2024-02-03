using AltV.Net.Client;
using Proton.Client.Core.Interfaces;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.NoClips.Scripts;

public sealed class NoClipScript : IStartup
{
    private readonly INoClip noClip;

    public NoClipScript(INoClip noClip)
    {
        this.noClip = noClip;
        Alt.OnPlayerDisconnect += HandlePlayerDisconnect;
        Alt.OnServer("noclip:start", HandleStart);
        Alt.OnServer("noclip:stop", HandleStop);
    }

    private void HandleStart()
    {
        noClip.Start();
    }

    private void HandleStop()
    {
        noClip.Stop();
    }

    private void HandlePlayerDisconnect()
    {
        if (noClip.IsStarted)
        {
            noClip.Stop();
        }
    }
}
