using Proton.Shared.Constants;

namespace Proton.Server.Resource.Features.Races.Abstractions;

public interface IRacePointResolver
{
    RaceType SupportedRaceType { get; }

    RacePointResolverOutput Resolve(RacePointResolverInput input);
}
