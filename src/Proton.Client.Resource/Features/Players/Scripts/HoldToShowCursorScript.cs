using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using CnR.Client.Features.Games.Abstractions;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;

namespace Proton.Client.Resource.Features.Players.Scripts;

public sealed class HoldToShowCursorScript(IUiView uiView, IGame game) : HostedService
{
    private bool focusing;

    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnKeyUp += OnKeyUp;
        Alt.OnWindowFocusChange += OnWindowFocusChange;
        return Task.CompletedTask;
    }

    private void OnKeyUp(Key key)
    {
        if (key == Key.Menu)
        {
            if (focusing)
            {
                uiView.Unfocus();
                game.ToggleCursor(false);
            }
            else
            {
                uiView.Focus();
                game.ToggleCursor(true);
            }
            focusing = !focusing;
        }
    }

    private void OnWindowFocusChange(bool state)
    {
        if (focusing)
        {
            uiView.Unfocus();
            game.ToggleCursor(false);
            focusing = false;
        }
    }
}
