using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Races.Scripts;

public sealed class RaceMenuScript : IStartup
{
    private readonly IUiView uiView;

    public RaceMenuScript(IUiView uiView)
    {
        this.uiView = uiView;
        Alt.OnKeyUp += HandleKeyUp;
        Alt.OnServer<long>("race:prepare", HandleServerPrepare);
        uiView.OnMount(Route.RaceMainMenuList, HandleOnMount);
        uiView.OnUnmount(Route.RaceMainMenuList, HandleOnUnmount);
    }

    private void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.Tab:
                if (uiView.IsMounted(Route.RaceMainMenuList))
                {
                    break;
                }

                uiView.Mount(Route.RaceMainMenuList);
                break;
            case Key.Escape:
                if (!uiView.IsMounted(Route.RaceMainMenuList))
                {
                    break;
                }

                uiView.Unmount(Route.RaceMainMenuList);
                break;
        }
    }

    private void HandleServerPrepare(long _)
    {
        if (uiView.IsMounted(Route.RaceMainMenuList))
        {
            uiView.Unmount(Route.RaceMainMenuList);
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
}
