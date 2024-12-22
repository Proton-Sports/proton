<<<<<<< HEAD
using AltV.Net;
=======
﻿using AltV.Net;
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
<<<<<<< HEAD
using Proton.Server.Core.Interfaces;
using Proton.Server.Core.Models;
=======
>>>>>>> fa66636fead8b440ddf791e624522826101fdfe1
using Proton.Server.Infrastructure.Authentication;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.SharedKernel;

namespace Proton.Server.Resource.Authentication.Scripts;

public class AuthenticationScript(
    DiscordHandler discord,
    IDbContextFactory dbContextFactory,
    IConfiguration configuration
) : HostedService
{
    public delegate Task OnAuthenticationDone(IPlayer p);
    public static event OnAuthenticationDone? OnAuthenticationDoneEvent;

    public delegate Task OnAuthenticationDone(IPlayer p);
    public static event OnAuthenticationDone? OnAuthenticationDoneEvent;

    private Dictionary<IPlayer, DiscordAccountHandler> playerAuthenticationStore =
        new Dictionary<IPlayer, DiscordAccountHandler>();

    public override Task StartAsync(CancellationToken ct)
    {
        //AltAsync.OnPlayerSpawn += OnPlayerConnect;
        AltAsync.OnPlayerConnect += OnPlayerConnect;
        AltAsync.OnPlayerDisconnect += OnPlayerDisconnect;
        AltAsync.OnResourceStop += OnResourceStop;
        Alt.OnClient<string>(
            "authentication:token:exchange",
            (player, token) => OnTokenExchange(player, token).GetAwaiter()
        );
        Alt.OnClient("authentication:login", (p) => OnPlayerWantsLogin(p));
        return Task.CompletedTask;
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
        var discordProfile = account.GetCurrentUser();

        await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        var isBanned = await db.BanRecords
            .AnyAsync(a => a.Kind == BanKind.Discord && a.Value.Equals(discordProfile.Id.ToString()))
            .ConfigureAwait(false);
        if (isBanned)
        {
            p.Kick("You were banned");
            return;
        }

        playerAuthenticationStore.Add(p, account);
        p.Emit("authentication:login:information", discordProfile.GetAvatarUrl(), p.Name);
    }

    private async Task OnPlayerWantsLogin(IPlayer _p)
    {
        var p = (PPlayer)_p;
        var account = playerAuthenticationStore[p]; //null checking
        if (!account.IsUserRegistered())
        {
            await account.Register(p.SocialClubName);
        }

        var id = await account.Login(p.Ip);
        if (id != 0)
        {
            await using var db = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
            var user = await db.Users
                .Where(a => a.Id == id)
                .Select(a => new { a.Role })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (user is null)
            {
                p.Kick("There was a mistake while logging in!");
                return;
            }
            p.ProtonId = id;
            p.Role = user.Role;
            p.Emit("authentication:login:ok");

            // OLD: For compatible
            p.SetStreamSyncedMetaData("playerName", p.SocialClubName);
            Alt.Emit("auth:firstSignIn", p);

            // NEW
            if (OnAuthenticationDoneEvent != null)
                await OnAuthenticationDoneEvent(p);
        }
        else
        {
            p.Kick("There was a mistake while logging in!");
        }
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
