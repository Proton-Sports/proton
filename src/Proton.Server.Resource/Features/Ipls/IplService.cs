using System.Collections.Concurrent;
using AltV.Net;
using AltV.Net.Elements.Entities;
using Proton.Server.Resource.Features.Ipls.Abstractions;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource.Features.Ipls;

public sealed class IplService : IIplService
{
    private readonly IIncrementalCounter counter;
    private readonly ConcurrentDictionary<long, LoadAsyncState> loadAsyncTaskDictionary = new();

    public IplService(IIncrementalCounter counter)
    {
        this.counter = counter;
        Alt.OnClient<IPlayer, long>("ipl:loadAsync", HandleClientLoadAsync);
    }

    public bool Load(IPlayer player, string name)
    {
        player.Emit("ipl:load", name);
        return true;
    }

    public Task<bool> LoadAsync(IEnumerable<IPlayer> players, string name)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var id = counter.GetNext();
        var source = new TaskCompletionSource<bool>();
        var playersArr = players.ToArray();
        loadAsyncTaskDictionary[id] = new LoadAsyncState { Source = source, Count = playersArr.Length };
        cts.Token.Register(() =>
        {
            if (loadAsyncTaskDictionary.TryRemove(id, out var state))
            {
                state.Source.SetResult(false);
            }
        });
        Alt.EmitClients(playersArr, "ipl:loadAsync", id, name);
        return source.Task;
    }

    public bool Unload(IPlayer player, string name)
    {
        player.Emit("ipl:unload", name);
        return true;
    }

    private void HandleClientLoadAsync(IPlayer _, long id)
    {
        if (!loadAsyncTaskDictionary.TryGetValue(id, out var state))
        {
            return;
        }

        if (--state.Count == 0)
        {
            state.Source.SetResult(true);
            loadAsyncTaskDictionary.Remove(id, out var _);
        }
    }

    private sealed class LoadAsyncState
    {
        public required TaskCompletionSource<bool> Source;
        public int Count;
    }
}
