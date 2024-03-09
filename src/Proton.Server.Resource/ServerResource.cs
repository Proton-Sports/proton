using System.Reflection;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Args;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proton.Shared.Adapters;
using Proton.Shared.Dtos;
using Proton.Server.Resource.Authentication.Extentions;
using Proton.Shared.Interfaces;
using Proton.Shared.Models;
using AltV.Net.Elements.Entities;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Shop.Extentions;

namespace Proton.Server.Resource;

public sealed class ServerResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;
    public ServerResource()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile("appsettings.Local.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        serviceProvider = serviceCollection
            .AddInfrastructure(configuration)
            //.AddRaces()
            .AddSingleton<IConfiguration>(configuration)
            //.AddAuthentication()
            .AddShops()
            .BuildServiceProvider();
    }

    public override void OnStart()
    {
        //Alt.RegisterMValueAdapter(SharedRaceCreatorDataMValueAdapter.Instance);
        //Alt.RegisterMValueAdapter(RaceMapDto.Adapter.Instance);
        //Alt.RegisterMValueAdapter(DefaultMValueAdapters.GetArrayAdapter(RaceMapDto.Adapter.Instance));
        Alt.RegisterMValueAdapter(MValueListAdapter<SharedShopItem, SharedShopItemAdapter>.Instance);

        AltExtensions.RegisterAdapters(true, true);
        // TODO: Add logging for startup
        var services = serviceProvider.GetServices<IStartup>();
    }

    public override IEntityFactory<IPlayer> GetPlayerFactory()
    {
        return new PPlayerFactory();
    }

    public override void OnStop() { }
}
