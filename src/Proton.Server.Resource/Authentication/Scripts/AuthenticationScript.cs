using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Proton.Server.Core.Interfaces;
using Proton.Server.Infrastructure.Authentication;
using Proton.Server.Infrastructure.Persistence;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Server.Resource.Authentication.Scripts;

public class AuthenticationScript : IStartup
{
    private readonly DiscordHandler discord;
    private readonly IDbContextFactory<DefaultDbContext> dbContextFactory;
    private readonly IConfiguration configuration;

    private Dictionary<IPlayer, DiscordAccountHandler> playerAuthenticationStore = new Dictionary<IPlayer, DiscordAccountHandler>();

    public AuthenticationScript(DiscordHandler discord, 
        IDbContextFactory<DefaultDbContext> dbContextFactory,
        IConfiguration configuration)
    {
        this.discord = discord;
        this.dbContextFactory = dbContextFactory;
        this.configuration = configuration;
        AltAsync.OnPlayerConnect += OnPlayerConnect;
        AltAsync.OnPlayerDisconnect += OnPlayerDisconnect;
        AltAsync.OnResourceStop += OnResourceStop;
        Alt.OnClient<string>("authentication:token:exchange", 
            (player, token) => OnTokenExchange(player, token).GetAwaiter());
        Alt.OnClient("authentication:login",
            (player) => OnPlayerWantsLogin(player).GetAwaiter());
    }    

    /// <summary>
    /// Checking if the OAuth Token is still valid and offer to login as User
    /// </summary>
    public Task OnPlayerConnect(IPlayer p, string reason)
    {
        string? appId = configuration["Discord:AppId"];
        p.Emit("authentication:token:check", appId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Logging the User out
    /// </summary>
    public async Task OnPlayerDisconnect(IPlayer p, string reason)
    {
        await playerAuthenticationStore[p].Logout();
        playerAuthenticationStore.Remove(p);
    }

    /// <summary>
    /// The client sends the Discord OAuth Challenge Token
    /// </summary>
    private async Task OnTokenExchange(IPlayer p, string token)
    {
        var account = await discord.GetAccountHandler(token, dbContextFactory);
        playerAuthenticationStore.Add(p, account);

        var discordProfile = account.GetCurrentUser();
        p.Emit("authentication:login:information", discordProfile.GetAvatarUrl());
    }

    private async Task OnPlayerWantsLogin(IPlayer p)
    {
        var account = playerAuthenticationStore[p]; //null checking
        if (!account.IsUserRegistered())
        {
            await account.Register(p.SocialClubName);
        }

        await account.Login(p.Ip);
    }

    /// <summary>
    /// Log all Users out if the server stopps
    /// </summary>
    private async Task OnResourceStop(INativeResource resource)
    {
        foreach (var k in playerAuthenticationStore)
        {
            await k.Value.Logout();
        }

        playerAuthenticationStore.Clear();
    }
}
