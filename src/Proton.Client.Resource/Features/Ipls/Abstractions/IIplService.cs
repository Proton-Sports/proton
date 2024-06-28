namespace Proton.Client.Resource.Features.Ipls.Abstractions;

public interface IIplService
{
    bool IsLoaded(string name);
    Task<bool> LoadAsync(string name);
    Task<bool> UnloadAsync(string name);
}
