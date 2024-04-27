using AltV.Net.Client;
using AltV.Net.Client.Elements.Entities;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Nametags.Scripts;

public class NametagsScript : IStartup
{
    private bool areNametagsShown = true;
    private readonly Dictionary<IEntity, ITextLabel> nametagsElements = new();
    private int everyTickEvent = -1;

    public NametagsScript()
    {
        Alt.RegisterFont("/fonts/arialbd.ttf");
        
        Alt.OnGameEntityCreate += OnGameEntityCreate;
        Alt.OnGameEntityDestroy += OnGameEntityDestroy;
        
        Alt.OnServer<bool>("clientNametags:showNametags", ShowNametags);
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
        var textLabel = Alt.CreateTextLabel("Test", "arial", 25, 1, entity.Position, entity.Rotation, new Rgba(255, 255, 255, 255), 0,
            Rgba.Zero, false, 50);
        textLabel.IsFacingCamera = true;
        
        nametagsElements.Add(entity, textLabel);
        
        if (everyTickEvent != -1) return;
        everyTickEvent = (int) Alt.EveryTick(DrawNametags);
    }

    private void DrawNametags()
    {
        if (!areNametagsShown) return;
        
        for (var index = 0; index < nametagsElements.Count; index++) {
            var (nametagEntity, nametagTextLabel) = nametagsElements.ElementAt(index);

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