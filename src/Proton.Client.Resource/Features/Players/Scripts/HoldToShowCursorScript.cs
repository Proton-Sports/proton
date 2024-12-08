using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;

namespace Proton.Client.Resource.Features.Players.Scripts;

public sealed class HoldToShowCursorScript(IUiView uiView) : HostedService
{
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
            if (Alt.IsCursorVisible)
            {
                uiView.Unfocus();
                Alt.ShowCursor(false);
            }
            else
            {
                uiView.Focus();
                Alt.ShowCursor(true);
            }
        }
    }

    private void OnWindowFocusChange(bool state)
    {
        if (Alt.IsCursorVisible)
        {
            uiView.Unfocus();
            Alt.ShowCursor(false);
        }
    }
}
