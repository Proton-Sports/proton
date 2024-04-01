using Microsoft.EntityFrameworkCore;
using Proton.Server.Infrastructure.Persistence;

namespace Proton.Server.Infrastructure.CharacterCreator;

public class CharacterHandler(IDbContextFactory<DefaultDbContext> defaultDbFactory)
{
    public async Task<bool> ExistsCharacter(int characterId)
    {
        var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.AnyAsync(x => x.Id == characterId);
    }
}