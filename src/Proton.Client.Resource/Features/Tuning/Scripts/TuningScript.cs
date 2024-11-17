using System.Text.Json;
using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;
using Proton.Shared.Dtos;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Features.Tuning.Scripts;

public sealed class TuningScript : IStartup
{
    private readonly IUiView uiView;
    private bool isOpen;
    internal Dictionary<string, List<TuningDetailDto>> tuneableItems = new Dictionary<string, List<TuningDetailDto>>();

    public TuningScript(IUiView uiView)
    {
        this.uiView = uiView;
        uiView.On("tuning:started", sendTuningItems);

        Alt.OnConsoleCommand += Alt_OnConsoleCommand;
        Alt.OnKeyDown += Alt_OnKeyDown;
        Alt.OnServer<TuningDto>("tuning:items:available", OnTuningItemsAvailable);
    }

    private void Alt_OnKeyDown(AltV.Net.Client.Elements.Data.Key key)
    {
        if(isOpen && key == AltV.Net.Client.Elements.Data.Key.Escape)
        {
            Alt.ShowCursor(false);
            Alt.GameControlsEnabled = true;
            uiView.Unmount(Route.TuningMenu);
            isOpen = false;
        }
    }

    private void sendTuningItems()
    {
        uiView.Emit("tuning:data", JsonSerializer.Serialize(tuneableItems));
        Alt.ShowCursor(true);
        Alt.GameControlsEnabled = false;
        uiView.Focus();
        isOpen = true;
    }

    private void OnTuningItemsAvailable(TuningDto items)
    {
        tuneableItems.Clear();
        foreach (var x in items.TuningWrappers)
        {
            tuneableItems.Add(x.Name, x.Tunings);
        }

        uiView.Mount(Route.TuningMenu);        
    }

    private void Alt_OnConsoleCommand(string name, string[] args)
    {
        if(name == "veh")
        {
            Alt.EmitServer("tune");
        }
    }
}
