using AltV.Net.Elements.Entities;
using Microsoft.Extensions.Options;
using Proton.Server.Resource.Authentication.Scripts;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Features.Ipls.Scripts;

public sealed class UnloadIplsScript(IOptionsMonitor<IplOptions> options) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        AuthenticationScript.OnAuthenticationDoneEvent += OnAuthenticationDone;
        return Task.CompletedTask;
    }

    private Task OnAuthenticationDone(IPlayer player)
    {
        player.Emit("ipl.unload", [options.CurrentValue.Entries.Select(a => a.Name).ToArray()]);
        return Task.CompletedTask;
    }
}
