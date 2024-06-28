using Proton.Server.Resource.Features.Races.Abstractions;
using Proton.Shared.Constants;

namespace Proton.Server.Resource.Features.Races;

public sealed class RacePointRallyResolver : IRacePointResolver
{
    public RaceType SupportedRaceType => RaceType.PointToPoint;

    public RacePointResolverOutput Resolve(RacePointResolverInput input)
    {
        var output = new RacePointResolverOutput() { Lap = input.Lap, Index = input.Index };

        if (output.Index == input.TotalPoints - 1)
        {
            output.Finished = true;
            return output;
        }

        ++output.Index;
        if (output.Index != input.TotalPoints - 1)
        {
            output.NextIndex = output.Index + 1;
        }
        return output;
    }
}
