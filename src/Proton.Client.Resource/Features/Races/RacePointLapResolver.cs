using Proton.Client.Resource.Features.Races.Models;
using Proton.Shared.Constants;

namespace Proton.Client.Resource.Features.Races;

public sealed class RacePointLapResolver : IRacePointResolver
{
    public RaceType SupportedRaceType => RaceType.Laps;

    public RacePointResolverOutput Resolve(RacePointResolverInput input)
    {
        var output = new RacePointResolverOutput()
        {
            Lap = input.Lap,
            Index = input.Index
        };
        if (output.Index == 0)
        {
            if (output.Lap == input.TotalLaps)
            {
                output.Finished = true;
                return output;
            }
            ++output.Lap;
        }
        output.Index = (output.Index + 1) % input.TotalPoints;

        // Not the final point
        if (output.Index != 0 || output.Lap != input.TotalLaps)
        {
            output.NextIndex = (output.Index + 1) % input.TotalPoints;
        }
        return output;
    }
}
