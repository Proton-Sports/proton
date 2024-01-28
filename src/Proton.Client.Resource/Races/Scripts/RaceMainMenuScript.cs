using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Races.Scripts;

public sealed class RaceMainMenuScript : IStartup
{
    private readonly IUiView uiView;
    private bool mounted = false;

    public RaceMainMenuScript(IUiView uiView)
    {
        this.uiView = uiView;
        Alt.OnKeyUp += HandleKeyUp;
    }

    private void HandleKeyUp(Key key)
    {
        switch (key)
        {
            case Key.Tab:
                if (mounted) break;
                mounted = true;
                uiView.Mount(Route.RaceMainMenu);
                uiView.Focus();
                Alt.ShowCursor(true);
                Alt.GameControlsEnabled = false;
                break;
            case Key.Escape:
                if (!mounted) break;
                mounted = false;
                uiView.Unmount(Route.RaceMainMenu);
                uiView.Unfocus();
                Alt.ShowCursor(false);
                Alt.GameControlsEnabled = true;
                break;
        }
    }
}
