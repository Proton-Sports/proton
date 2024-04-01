using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;

namespace Proton.Client.Resource.Features.Races.Abstractions;

public class RacePointData : BasePositionData
{
    public ICheckpoint Checkpoint { get; set; }

    public RacePointData(Position position, ICheckpoint checkpoint, IBlip blip) : base(position, blip)
    {
        Checkpoint = checkpoint;
    }

    public override void Destroy()
    {
        base.Destroy();
        Checkpoint.Destroy();
    }
}
