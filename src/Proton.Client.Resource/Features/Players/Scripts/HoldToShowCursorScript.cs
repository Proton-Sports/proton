using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;

namespace Proton.Client.Resource.Features.Players.Scripts;

public sealed class HoldToShowCursorScript(IUiView uiView) : HostedService
{
    public override Task StartAsync(CancellationToken ct)
    {
        Alt.OnKeyDown += OnKeyDown;
        Alt.OnKeyUp += OnKeyUp;
        return Task.CompletedTask;
    }

    private void OnKeyDown(Key key)
    {
        if (key == Key.Menu)
        {
            uiView.Focus();
            Alt.ShowCursor(true);
        }
    }

    private void OnKeyUp(Key key)
    {
        if (key == Key.Menu)
        {
            uiView.Unfocus();
            Alt.ShowCursor(false);
        }
    }
}
