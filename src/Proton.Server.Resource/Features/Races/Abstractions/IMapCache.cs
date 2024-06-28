using Proton.Server.Core.Models;

namespace Proton.Server.Resource.Features.Races.Abstractions;

public interface IMapCache
{
    Task<RaceMap?> GetAsync(long id);
}
