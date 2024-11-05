using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.Races.Abstractions;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceMenuScript(IUiView uiView, IRaceService raceService) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnKeyUp += HandleKeyUp;
        Alt.OnServer<long>("race:prepare", HandleServerPrepare);
        uiView.OnMount(Route.RaceMenu, HandleOnMount);
        uiView.OnUnmount(Route.RaceMenu, HandleOnUnmount);
        Alt.OnServer<RaceStartDto>("race-start:start", (_) => ToggleCollectionPageConditionally(false));
        Alt.OnServer<Vector3>("race-prepare:enterTransition", (_) => ToggleCollectionPageConditionally(false));
        Alt.OnServer("race:destroy", () => ToggleCollectionPageConditionally(true));
        uiView.On("race-menu.tokens.get", OnUiGetToken);
        Alt.OnServer<int>("race-menu.tokens.get", OnServerGetToken);
        return Task.CompletedTask;
    }

    private void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.Tab:
                if (uiView.IsMounted(Route.RaceMenu))
                {
                    break;
                }

                uiView.Mount(
                    Route.RaceMenu,
                    new RaceMenuMountDto
                    {
                        InitialDisabledPages = raceService.Status == RaceStatus.None ? null : new() { "collection" }
                    }
                );
                break;
            case Key.Escape:
                if (!uiView.IsMounted(Route.RaceMenu))
                {
                    break;
                }

                uiView.Unmount(Route.RaceMenu);
                break;
        }
    }

    private void HandleServerPrepare(long _)
    {
        if (uiView.IsMounted(Route.RaceMenu))
        {
            uiView.Unmount(Route.RaceMenu);
        }
    }

    private void HandleOnMount()
    {
        Alt.GameControlsEnabled = false;
        Alt.ShowCursor(true);
        uiView.Focus();
    }

    private void HandleOnUnmount()
    {
        Alt.GameControlsEnabled = true;
        Alt.ShowCursor(false);
        uiView.Unfocus();
    }

    private void ToggleCollectionPageConditionally(bool toggle)
    {
        if (!uiView.IsMounted(Route.RaceMenu))
        {
            return;
        }

        uiView.Emit("race-menu.pages.toggle", "collection", toggle);
    }

    private void OnUiGetToken()
    {
        Alt.EmitServer("race-menu.tokens.get");
    }

    private void OnServerGetToken(int tokens)
    {
        uiView.Emit("race-menu.tokens.get", tokens);
    }
}
