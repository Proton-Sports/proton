using AltV.Net;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Proton.Server.Core.Models;
using Proton.Server.Infrastructure.Persistence;

namespace Proton.Server.Infrastructure.CharacterCreator;

public class CharacterHandler(IDbContextFactory<DefaultDbContext> defaultDbFactory)
{
    public async Task<bool> ExistsCharacter(int characterId)
    {
        await using var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.AnyAsync(x => x.Id == characterId);
    }

    public async Task<bool> HasCharacter(long userId)
    {
        await using var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.AnyAsync(x => x.UserId == userId);
    }
    
    public async Task<Character?> GetByUserId(long userId)
    {
        await using var defaultDb = await defaultDbFactory.CreateDbContextAsync();
        return await defaultDb.Characters.FirstOrDefaultAsync(x => x.UserId == userId);
    }
    
    public async Task Add(Character userCharacter)
    {
        try
        {
            await using var defaultDb = await defaultDbFactory.CreateDbContextAsync();
            defaultDb.Characters.Add(userCharacter);
            await defaultDb.SaveChangesAsync();
        } catch (DbUpdateException ex)
        {
            Alt.Log(ex.Message);
        }
    }
}