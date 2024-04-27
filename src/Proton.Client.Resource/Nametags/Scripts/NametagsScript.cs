using AltV.Net.Client;
using AltV.Net.Client.Elements.Entities;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Nametags.Scripts;

public class NametagsScript : IStartup
{
    private readonly IUiView uiView;
    private bool areNametagsShown;
    private bool clientSettingValue;
    private readonly Dictionary<IEntity, ITextLabel> nametagsElements = new();
    private int everyTickEvent = -1;

    public NametagsScript(IUiView uiView)
    {
        this.uiView = uiView;
        this.uiView.On("nametagsClient:getSetting", GetSetting);
        this.uiView.On<bool>("nametagsClient:setSetting", SetSetting);
        
        Alt.RegisterFont("/fonts/arialbd.ttf");
        
        Alt.OnGameEntityCreate += OnGameEntityCreate;
        Alt.OnGameEntityDestroy += OnGameEntityDestroy;
        Alt.OnStreamSyncedMetaChange += OnStreamSyncedMetaChange;
        
        Alt.OnServer<bool>("clientNametags:showNametags", ShowNametags);
        
        var hasShowNametags = Alt.LocalStorage.Has("showNametags");
        if (!hasShowNametags)
        {
            Alt.LocalStorage.Set("showNametags", true);
            Alt.LocalStorage.Save();

            clientSettingValue = true;
        }
        else
        {
            Alt.LocalStorage.Get("showNametags", out bool showNametags);
            clientSettingValue = showNametags;
        }
    }

    private void OnStreamSyncedMetaChange(IBaseObject target, string key, object value, object oldvalue)
    {
        if (target is not IEntity targetAsEntity) return;
        if (key != "playerName") return;
        
        var containsKey = nametagsElements.ContainsKey(targetAsEntity);
        if (!containsKey) return;

        nametagsElements.TryGetValue(targetAsEntity, out var targetLabel);
        if (targetLabel == null) return;
        targetLabel.Text = value as string;
    }

    private void SetSetting(bool settingValue)
    {
        Alt.LocalStorage.Set("showNametags", settingValue);
        Alt.LocalStorage.Save();

        clientSettingValue = settingValue;
    }

    private void GetSetting()
    {
        var hasShowNametags = Alt.LocalStorage.Has("showNametags");
        if (!hasShowNametags)
        {
            Alt.LocalStorage.Set("showNametags", true);
            Alt.LocalStorage.Save();
            
            uiView.Emit("settings-nametags:setValue", true);
        }
        else
        {
            Alt.LocalStorage.Get("showNametags", out bool showNametags);
            uiView.Emit("settings-nametags:setValue", showNametags);
        }
    }

    private void ShowNametags(bool toggleValue)
    {
        areNametagsShown = toggleValue;
    }
    
    private void OnGameEntityDestroy(IEntity entity)
    {
        if (entity is not Player) return;
        nametagsElements.TryGetValue(entity, out var entityTextLabel);

        if (entityTextLabel == null) return;
        entityTextLabel.Destroy();
        
        nametagsElements.Remove(entity);

        if (nametagsElements.Count != 0) return;
        if (everyTickEvent == -1) return;

        try
        {
            Alt.ClearEveryTick((uint) everyTickEvent);
            everyTickEvent = -1;
        }
        catch (Exception exception)
        {
            Alt.LogError(exception.Message);
        }
    }

    private void OnGameEntityCreate(IEntity entity)
    {
        if (entity is not Player) return;
        entity.GetStreamSyncedMetaData("playerName", out string playerName);
        
        var textLabel = Alt.CreateTextLabel(playerName, "arial", 25, 1, entity.Position, entity.Rotation, new Rgba(255, 255, 255, 255), 0,
            Rgba.Zero, false, 50);
        textLabel.IsFacingCamera = true;
        
        nametagsElements.Add(entity, textLabel);
        
        if (everyTickEvent != -1) return;
        everyTickEvent = (int) Alt.EveryTick(DrawNametags);
    }

    private void DrawNametags()
    {
        foreach (var (nametagEntity, nametagTextLabel) in nametagsElements)
        {
            if (!areNametagsShown || !clientSettingValue)
            {
                if (!nametagTextLabel.Visible) return;
                
                nametagTextLabel.Visible = false;
                return;
            }

            var distanceBetween = Alt.LocalPlayer.Position.Distance(nametagEntity.Position);
            if (distanceBetween > 15)
            {
                nametagTextLabel.Visible = false;
                return;
            }
            
            var isPointOnScreen = Alt.IsPointOnScreen(nametagEntity.Position);
            if (!isPointOnScreen)
            {
                nametagTextLabel.Visible = false;
                return;
            }
            
            nametagTextLabel.Position = new Position(nametagEntity.Position.X, nametagEntity.Position.Y,
                nametagEntity.Position.Z + 1);
            nametagTextLabel.Dimension = nametagEntity.Dimension;
            nametagTextLabel.Visible = true;
            
            var distance = Alt.LocalPlayer.Position.Distance(nametagTextLabel.Position);
            var normalizedDistance = Math.Min(1, distance / 15);
            var alphaValue = (byte)(255 * (1 - normalizedDistance));

            nametagTextLabel.Color = new Rgba(255, 255, 255, alphaValue);
        }
    }
}