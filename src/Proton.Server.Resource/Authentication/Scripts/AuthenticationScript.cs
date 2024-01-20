using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Authentication;
using Proton.Server.Infrastructure.Persistence;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Authentication.Scripts;

public class AuthenticationScript : IStartup
{
    private readonly DiscordHandler discord;
    private readonly IDbContextFactory<DefaultDbContext> dbContextFactory;
    private readonly DefaultDbContext dbContext;

    public AuthenticationScript(DiscordHandler discord, IDbContextFactory<DefaultDbContext> dbContextFactory)
    {
        this.discord = discord;
        this.dbContextFactory = dbContextFactory;

        dbContext = dbContextFactory.CreateDbContext();
        AltAsync.OnPlayerConnect += OnPlayerConnect;
    }

    /// <summary>
    /// Checking if the OAuth Token is still valid and offer to login as User
    /// </summary>
    public async Task OnPlayerConnect(IPlayer p, string reason)
    {
        Alt.Log(dbContext.Users.Count().ToString());
        Alt.LogInfo("player joined");
    }

    /// <summary>
    /// The client sends the Discord OAuth Challenge Token
    /// </summary>
    [AsyncClientEvent("authentication:token:exchange")]
    public async Task OnTokenExchange(IPlayer p, string token)
    {

    }
}
