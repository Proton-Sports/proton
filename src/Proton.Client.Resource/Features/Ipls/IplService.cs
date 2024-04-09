using AltV.Net.Client;
using AltV.Net.Client.Async;
using Proton.Client.Resource.Features.Ipls.Abstractions;

namespace Proton.Client.Resource.Features.Ipls;

public sealed class IplService : IIplService
{
    public IplService()
    {
        Alt.OnServer<string>("ipl:load", HandleLoadIpl);
        Alt.OnServer<long, string, Task>("ipl:loadAsync", HandleLoadIplAsync);
        Alt.OnServer<string>("ipl:unload", HandleUnloadIpl);
    }

    public bool IsLoaded(string name)
    {
        return Alt.Natives.IsIplActive(name);
    }

    public async Task<bool> LoadAsync(string name)
    {
        Alt.RequestIpl(name);
        try
        {
            await AltAsync.WaitFor(() => Alt.Natives.IsIplActive(name), timeout: 3000, interval: 30);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task<bool> UnloadAsync(string name)
    {
        Alt.RemoveIpl(name);
        try
        {
            await AltAsync.WaitFor(() => !Alt.Natives.IsIplActive(name), timeout: 3000, interval: 30);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    private void HandleLoadIpl(string name)
    {
        Alt.RequestIpl(name);
    }

    private void HandleUnloadIpl(string name)
    {
        Alt.RemoveIpl(name);
    }

    private async Task HandleLoadIplAsync(long id, string name)
    {
        await LoadAsync(name).ConfigureAwait(false);
        Alt.EmitServer("ipl:loadAsync", id);
    }
}
