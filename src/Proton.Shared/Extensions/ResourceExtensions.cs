using AltV.Net;
using AltV.Net.Elements.Args;
using AltV.Net.Shared;
using Proton.Shared.Adapters;
using Proton.Shared.Dtos;

namespace Proton.Shared.Extensions;

public sealed class ResourceExtensions
{
    public static void RegisterMValueAdapters()
    {
        AltShared.Core.RegisterMValueAdapter(SharedRaceCreatorDataMValueAdapter.Instance);
        AltShared.Core.RegisterMValueAdapter(RaceMapDto.Adapter.Instance);
        AltShared.Core.RegisterMValueAdapter(DefaultMValueAdapters.GetArrayAdapter(RaceMapDto.Adapter.Instance));
        AltShared.Core.RegisterMValueAdapter(DefaultMValueAdapters.GetArrayAdapter(SharedRacePointMValueAdapter.Instance));
        AltShared.Core.RegisterMValueAdapter(RaceHostSubmitDto.Adapter.Instance);
        AltExtensions.RegisterAdapters(registerListAdapters: true, logAdapters: true);
    }
}
