using AltV.Net.Client;
using AltV.Net.Client.Elements.Entities;
using Proton.Client.Infrastructure.Interfaces;
using Proton.Shared.Contants;

namespace Proton.Client.Infrastructure.Services;

public class DefaultUiView : WebView, IUiView
{
    private readonly Dictionary<string, TaskCompletionSource<bool>> mountTaskDictionary = new();
    private readonly Dictionary<string, bool> mountedDictionary = new();

    public DefaultUiView(ICore core, IntPtr webViewNativePointer, uint id) : base(core, webViewNativePointer, id)
    {
        this.On<string, bool>("webview.mount", HandleMount);
    }

    public bool IsMounted(Route route)
    {
        return mountedDictionary.ContainsKey(route.Value);
    }

    public void Mount(Route route)
    {
        Emit("webview.mount", route.Value);
        mountedDictionary.TryAdd(route.Value, true);
    }

    public Task<bool> TryMountAsync(Route route)
    {
        if (mountTaskDictionary.TryGetValue(route.Value, out var taskCompletionSource))
        {
            return taskCompletionSource.Task;
        }

        taskCompletionSource = new TaskCompletionSource<bool>();
        mountTaskDictionary[route.Value] = taskCompletionSource;
        Mount(route);
        return taskCompletionSource.Task;
    }

    public void Unmount(Route route)
    {
        Emit("webview.unmount", route.Value);
        mountedDictionary.Remove(route.Value);
    }

    private void HandleMount(string route, bool success)
    {
        if (!mountTaskDictionary.TryGetValue(route, out var taskCompletionSource)) return;
        taskCompletionSource.SetResult(success);
        mountTaskDictionary.Remove(route);
    }
}
