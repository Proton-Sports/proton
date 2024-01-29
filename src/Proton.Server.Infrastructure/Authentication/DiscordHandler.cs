using Discord.Rest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Proton.Server.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Infrastructure.Authentication
{
    public class DiscordHandler
    {
        public async Task<DiscordAccountHandler> GetAccountHandler(string Token, IDbContextFactory<DefaultDbContext> defaultDbFactory)
        {
            var client = new DiscordRestClient();
            await client.LoginAsync(Discord.TokenType.Bearer, Token);
            return new DiscordAccountHandler(client, defaultDbFactory);
        }
    }
}
