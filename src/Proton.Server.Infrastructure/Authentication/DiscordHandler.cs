using Discord.Rest;
using Proton.Server.Core.Interfaces;

namespace Proton.Server.Infrastructure.Authentication
{
    public class DiscordHandler
    {
        public async Task<DiscordAccountHandler> GetAccountHandler(string Token, IDbContextFactory defaultDbFactory)
        {
            var client = new DiscordRestClient();
            await client.LoginAsync(Discord.TokenType.Bearer, Token);
            return new DiscordAccountHandler(client, defaultDbFactory);
        }
    }
}
