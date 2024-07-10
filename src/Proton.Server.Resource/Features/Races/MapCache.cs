using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
using Proton.Server.Resource.Features.Races.Abstractions;

namespace Proton.Server.Resource.Features.Races;

public sealed class MapCache(IDbContextFactory dbContextFactory, IMemoryCache cache) : IMapCache
{
    public Task<RaceMap?> GetAsync(long id)
    {
        return cache.GetOrCreateAsync($"RaceMap:{id}", (_) => GetFromDBAsync(id));
    }

    public async Task<RaceMap?> GetFromDBAsync(long id)
    {
        await using var ctx = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var map = await ctx
            .RaceMaps.Where(x => x.Id == id)
<<<<<<< HEAD
            .Include(x => x.RacePoints.OrderBy(x => x.Index))
            .AsSplitQuery()
            .Include(x => x.StartPoints.OrderBy(x => x.Index))
=======
            .Include(x => x.RacePoints.OrderBy(x => x.Index).ToArray())
            .AsSplitQuery()
            .Include(x => x.StartPoints.OrderBy(x => x.Index).ToArray())
>>>>>>> main
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        return map;
    }
}
