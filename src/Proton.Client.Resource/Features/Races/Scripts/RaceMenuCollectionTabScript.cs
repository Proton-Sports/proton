using AltV.Net.Client;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceMenuCollectionTabScript(IUiView ui) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        ui.On<string>("race-menu-collection.option.change", OnOptionChange);
        return Task.CompletedTask;
    }

    private void OnOptionChange(string option)
    {
        if (!option.Equals("cars") && !option.Equals("clothes"))
        {
            return;
        }

        ui.Unmount(Route.RaceMainMenuList);
        Alt.EmitServer("race-menu-collection.option.change", option);
    }
}
