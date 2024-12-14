using AltV.Net.Client;
using Proton.Client.Resource.Commons;

namespace Proton.Client.Resource.Features.Ipls.Scripts;

public sealed class UnloadIplsScript : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnServer<string[]>("ipl.unload", UnloadIpls);
        return Task.CompletedTask;
    }

    private void UnloadIpls(string[] names)
    {
        foreach (var name in names)
        {
            Alt.RemoveIpl(name);
        }
    }
}
