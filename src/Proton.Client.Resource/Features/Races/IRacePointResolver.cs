using Proton.Client.Resource.Features.Races.Models;
using Proton.Shared.Constants;

namespace Proton.Client.Resource.Features.Races;

public interface IRacePointResolver
{
    RaceType SupportedRaceType { get; }

    RacePointResolverOutput Resolve(RacePointResolverInput input);
}
