﻿using AltV.Net.Client;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Interfaces;

namespace Proton.Client.Resource.Authentication.Scripts;

public sealed class AuthenticationScript : IStartup
{
    private readonly IUiView uiView;

    public AuthenticationScript(IUiView uiView)
    {
        Alt.OnServer<string>("authentication:token:check", (appId) => OnAuthenticationCheck(appId).GetAwaiter());
        Alt.OnServer<string, string>(
            "authentication:login:information",
            (avatar, name) => OnProfileInformation(avatar, name)
        );
        Alt.OnServer("authentication:login:ok", () => LoginOk());

        this.uiView = uiView;

        this.uiView.On("authentication:login", SendLoginRequest);
        this.uiView.On("webview:ready", () => uiView.Mount(Route.Auth));
        this.uiView.Mounting += HandleMounting;
    }

    private void HandleMounting(Route route, MountingEventArgs e)
    {
        if (route == Route.RaceMenu && uiView.IsMounted(Route.Auth))
        {
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Checking if the OAuth Token is still valid and offer to login as User
    /// </summary>
    public Task OnAuthenticationCheck(string AppId)
    {
        uiView.OnMount(
            Route.Auth,
            async () =>
            {
                uiView.Focus();
                uiView.Visible = true;

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
        );
        return Task.CompletedTask;
    }

    public Task OnProfileInformation(string AvatarUri, string Username)
    {
        uiView.Emit("authentication:information", AvatarUri, Username);
        Alt.ShowCursor(true);
        Alt.GameControlsEnabled = false;
        return Task.CompletedTask;
    }

    public Task SendLoginRequest()
    {
        Alt.EmitServer("authentication:login");
        return Task.CompletedTask;
    }

    public Task LoginOk()
    {
        Alt.ShowCursor(false);
        Alt.GameControlsEnabled = true;
        uiView.Unfocus();
        uiView.Unmount(Route.Auth);
        Alt.EmitClient("authentication:done");
        return Task.CompletedTask;
    }
}
