using Discord.Rest;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Models;
using Proton.Server.Core.Models.Log;
using Proton.Server.Infrastructure.Persistence;

namespace Proton.Server.Infrastructure.Authentication
{
    public class DiscordAccountHandler(
        DiscordRestClient restClient,
        IDbContextFactory<DefaultDbContext> defaultDbFactory
    )
    {
        private readonly DiscordRestClient restClient = restClient;
        private readonly IDbContextFactory<DefaultDbContext> defaultDbFactory = defaultDbFactory;

        public RestSelfUser GetCurrentUser()
        {
            return restClient.CurrentUser;
        }

        public bool IsUserRegistered()
        {
            var defaultDb = defaultDbFactory.CreateDbContext();
            return defaultDb.Users.Where(x => x.DiscordId == GetCurrentUser().Id).Any();
        }

        public async Task<long> Login(string Ip)
        {
            var defaultDb = defaultDbFactory.CreateDbContext();
            string[] ipSplited = Ip.Split(':');
            var user = defaultDb.Users.Where(x => x.DiscordId == GetCurrentUser().Id).Single();
            if (user is null)
            {
                await Console.Out.WriteLineAsync("user null");
                return 0;
            }
            else
            {
                string IPv6 = Ip.Replace(ipSplited.Last().ToString(), "");
                defaultDb.Sessions.Add(
                    new Session
                    {
                        UserId = user.Id,
                        Ipv4 = ipSplited.Last().ToString(),
                        Ipv6 = IPv6
                    }
                );

                await defaultDb.SaveChangesAsync();
                return user.Id;
            }
        }

        public async Task Register(string Username)
        {
            var defaultDb = defaultDbFactory.CreateDbContext();
            defaultDb.Users.Add(new User { DiscordId = GetCurrentUser().Id, Username = Username });

            await defaultDb.SaveChangesAsync();
        }

        public async Task Logout()
        {
            var defaultDb = defaultDbFactory.CreateDbContext();
            var user = defaultDb.Users.Where(x => x.DiscordId == GetCurrentUser().Id).Single();
            if (user is null)
            {
                await Console.Out.WriteLineAsync("user null");
            }
            else
            {
                var activeSession = defaultDb
                    .Sessions.Where(x => x.UserId == user.Id && x.IsActive)
                    .OrderByDescending(x => x.Id)
                    .First();
                if (activeSession is not null)
                {
                    activeSession.TimestampLogout = DateTime.UtcNow;
                    activeSession.IsActive = false;
                    defaultDb.Sessions.Update(activeSession);
                    await defaultDb.SaveChangesAsync();
                }
                else
                {
                    await Console.Out.WriteLineAsync("no session found");
                }
            }
        }
    }
}
