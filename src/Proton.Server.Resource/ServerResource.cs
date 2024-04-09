using System.Reflection;
using AltV.Net.Async;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proton.Server.Resource.Authentication.Extentions;
using Proton.Shared.Interfaces;
using AltV.Net.Elements.Entities;
using Proton.Server.Infrastructure.Factorys;
using Proton.Shared.Extensions;
using AltV.Net;
using Proton.Server.Resource.CharacterCreator.Extensions;
using AltV.Net.Enums;
using AltV.Net.Data;

namespace Proton.Server.Resource;

public sealed class ServerResource : AsyncResource
{
    private readonly IServiceProvider serviceProvider = null!;
    public ServerResource()
    {
        var serviceCollection = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        serviceProvider = serviceCollection
            .AddInfrastructure()
            .AddSingleton<IConfiguration>(configuration)
            .AddAuthentication()
            .AddRaceFeatures()
            .AddCharacterCreator()
            .AddIplFeatures()
            .BuildServiceProvider();
    }

    public override void OnStart()
    {
        ResourceExtensions.RegisterMValueAdapters();

        // TODO: Add logging for startup
        var services = serviceProvider.GetServices<IStartup>();

        // Alt.OnConsoleCommand += (command, args) =>
        // {
        //     if (command == "load")
        //     {
        //         Alt.EmitAllClients("ipl:load", "sport_zancudo_01");
        //     }
        //     if (command == "unload")
        //     {
        //         Alt.EmitAllClients("ipl:unload", "sport_zancudo_01");
        //     }
        // };
        // Alt.OnPlayerConnect += (player, reason) =>
        // {
        //     player.SetDateTime(DateTime.Now);
        //     player.Model = (uint)PedModel.FreemodeMale01;
        //     var pos = new Position(-2492.15723f, 2689.836f, 4.74698734f);
        //     player.Spawn(pos);

        //     var veh = Alt.CreateVehicle(VehicleModel.Oppressor2, pos, new Rotation(0, 0, 0));
        //     player.SetIntoVehicle(veh, 1);
        // };
    }

    public override IEntityFactory<IPlayer> GetPlayerFactory()
    {
        return new PPlayerFactory();
    }

    public override void OnStop() { }
}
