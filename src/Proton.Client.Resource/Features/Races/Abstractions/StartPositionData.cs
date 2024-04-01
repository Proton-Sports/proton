using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;

namespace Proton.Client.Resource.Features.Races.Abstractions;

public class StartPositionData : BasePositionData
{
    public IMarker NumberMarker { get; set; }
    public IMarker BoxMarker { get; set; }
    public Rotation Rotation { get; set; }

    public StartPositionData(Position position, Rotation rotation, IMarker numberMarker, IMarker boxMarker, IBlip blip) : base(position, blip)
    {
        Rotation = rotation;
        NumberMarker = numberMarker;
        BoxMarker = boxMarker;
    }

    public override void Destroy()
    {
        base.Destroy();
        NumberMarker.Destroy();
        BoxMarker.Destroy();
    }
}
