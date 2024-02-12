using AltV.Net;
using AltV.Net.Client;
using AltV.Net.Client.Async;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proton.Client.Resource.Authentication.Scripts;

public sealed class AuthenticationScript : IStartup
{
    private readonly IUiView uiView;

    public AuthenticationScript(IUiView uiView)
    {
        Alt.OnServer<string>("authentication:token:check", 
            (appId) => OnAuthenticationCheck(appId).GetAwaiter());
        Alt.OnServer<string, string>("authentication:login:information", 
            (avatar, name) => OnProfileInformation(avatar, name));

        this.uiView = uiView;
    }

    /// <summary>
    /// Checking if the OAuth Token is still valid and offer to login as User
    /// </summary>
    public async Task OnAuthenticationCheck(string AppId)
    {
        Alt.LogInfo($"[AUTH] Player Request OAuth2Token, AppId: {AppId}");
        try
        {
            string token = await Alt.Discord.RequestOAuth2Token(AppId);
            Alt.LogInfo($"[AUTH] Token: {token}");
            Alt.EmitServer("authentication:token:exchange", token);
        }
        catch (Exception ex)
        {
            Alt.Log(ex.Message);
            Alt.Log(ex.Source);
            Alt.Log(ex.StackTrace);
        }
    }

    public Task OnProfileInformation(string AvatarUri, string Username)
    {
        uiView.Emit("authentication:information", AvatarUri, Username);
        return Task.CompletedTask;
    }
}
