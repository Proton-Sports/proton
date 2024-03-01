using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Infrastructure.Interfaces;
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
    }

    private void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.Tab:
                if (uiView.IsMounted(Route.RaceMainMenuList)) break;
                Mount();
                break;
            case Key.Escape:
                if (!uiView.IsMounted(Route.RaceMainMenuList)) break;
                Unmount();
                break;
        }
    }

    private void HandleServerPrepare(long _)
    {
        if (uiView.IsMounted(Route.RaceMainMenuList))
        {
            Unmount();
        }
    }

    private void Mount()
    {
        Alt.GameControlsEnabled = false;
        Alt.ShowCursor(true);
        uiView.Focus();
        uiView.Mount(Route.RaceMainMenuList);
    }

    private void Unmount()
    {
        Alt.GameControlsEnabled = true;
        Alt.ShowCursor(false);
        uiView.Unfocus();
        uiView.Unmount(Route.RaceMainMenuList);
    }
}
