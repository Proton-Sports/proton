using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Persistence;

namespace Proton.Server.Infrastructure.CharacterCreator;

public class CharacterHandler(IDbContextFactory<DefaultDbContext> defaultDbFactory)
{
    public async Task<bool> ExistsCharacter(int characterId)
    {
        var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.AnyAsync(x => x.Id == characterId);
    }

    public async Task<bool> HasCharacter(long userId)
    {
        var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.AnyAsync(x => x.Id == userId);
    }
    
    public async Task<Character?> GetByUserId(long userId)
    {
        var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.FirstOrDefaultAsync(x => x.Id == userId);
    }
    
    public async Task Add(Character userCharacter)
    {
        var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        defaultDb.Characters.Add(userCharacter);
        await defaultDb.SaveChangesAsync();
    }
}