using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;

namespace Proton.Client.Resource.Features.Races.Abstractions;

public abstract class BasePositionData
{
    public IBlip Blip { get; set; }
    public Position Position { get; set; }

    public BasePositionData(Position position, IBlip blip)
    {
        Position = position;
        Blip = blip;
    }

    public virtual void Destroy()
    {
        Blip.Destroy();
    }
}
