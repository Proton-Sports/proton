using AltV.Net.Client;
using AltV.Net.Client.Elements.Entities;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Contants;

namespace Proton.Client.Resource.Features.UiViews;

public class DefaultUiView : WebView, IUiView
{
    private readonly Dictionary<string, TaskCompletionSource<bool>> mountTaskDictionary = new();
    private readonly Dictionary<string, bool> mountedDictionary = new();
    private readonly Dictionary<string, HashSet<Action>> onMountHandlers = new();
    private readonly Dictionary<string, HashSet<Action>> onUnmountHandlers = new();

    public DefaultUiView(ICore core, IntPtr webViewNativePointer, uint id) : base(core, webViewNativePointer, id)
    {
        this.On<string, bool, bool>("webview.mount", HandleMount);
        this.On<string, bool, bool>("webview.unmount", HandleUnmount);
    }

    public event Action<Route, MountingEventArgs>? OnMounting;

    public bool IsMounted(Route route)
    {
        return mountedDictionary.ContainsKey(route.Value);
    }

    public void Mount(Route route)
    {
        if (OnMounting is not null)
        {
            var eventArgs = new MountingEventArgs { Cancel = false };
            OnMounting(route, eventArgs);
            if (eventArgs.Cancel)
            {
                return;
            }
        }
        Emit("webview.mount", route.Value);
    }

    public Action OnMount(Route route, Action callback)
    {
        if (!onMountHandlers.TryGetValue(route.Value, out var handlers))
        {
            handlers = new() { callback };
            onMountHandlers[route.Value] = handlers;
        }
        else
        {
            handlers.Add(callback);
        }
        return () =>
        {
            handlers.Remove(callback);
        };
    }

    public Action OnUnmount(Route route, Action callback)
    {
        if (!onUnmountHandlers.TryGetValue(route.Value, out var handlers))
        {
            handlers = new() { callback };
            onUnmountHandlers[route.Value] = handlers;
        }
        else
        {
            handlers.Add(callback);
        }
        return () =>
        {
            handlers.Remove(callback);
        };
    }

    public Task<bool> TryMountAsync(Route route)
    {
        if (OnMounting is not null)
        {
            var eventArgs = new MountingEventArgs { Cancel = false };
            OnMounting(route, eventArgs);
            if (eventArgs.Cancel)
            {
                return Task.FromResult(false);
            }
        }

        if (mountTaskDictionary.TryGetValue(route.Value, out var taskCompletionSource))
        {
            return taskCompletionSource.Task;
        }

        taskCompletionSource = new TaskCompletionSource<bool>();
        mountTaskDictionary[route.Value] = taskCompletionSource;
        Emit("webview.mount", route.Value);
        return taskCompletionSource.Task;
    }

    public void Unmount(Route route)
    {
        Emit("webview.unmount", route.Value);
        mountedDictionary.Remove(route.Value);
    }

    private void HandleMount(string route, bool success, bool emitHandlers)
    {
        if (success)
        {
            mountedDictionary.TryAdd(route, true);
            if (emitHandlers && onMountHandlers.TryGetValue(route, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler();
                }
            }
        }
        if (!mountTaskDictionary.TryGetValue(route, out var taskCompletionSource)) return;
        taskCompletionSource.SetResult(success);
        mountTaskDictionary.Remove(route);
    }

    private void HandleUnmount(string route, bool success, bool emitHandlers)
    {
        if (success)
        {
            mountedDictionary.Remove(route);
            if (emitHandlers && onUnmountHandlers.TryGetValue(route, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler();
                }
            }
        }
    }
}
