using AltV.Net.Client.Elements.Interfaces;
using Proton.Shared.Constants;

namespace Proton.Client.Resource.Features.UiViews.Abstractions;

public interface IUiView : IWebView
{
    // Called before a route is mounted, can be cancelled
    event Action<Route, MountingEventArgs> Mounting;

    void Mount(Route route);
    void Mount<T>(Route route, T data);
    Task<bool> TryMountAsync(Route route);
    void Unmount(Route route);
    bool IsMounted(Route route);
    Action OnMount(Route route, Action callback);
    Action OnUnmount(Route route, Action callback);
}
