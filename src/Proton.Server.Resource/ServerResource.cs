using System.Reflection;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proton.Server.Infrastructure.Factorys;
using Proton.Server.Resource.Authentication.Extentions;
using Proton.Server.Resource.CharacterCreator.Extensions;
using Proton.Shared.Extensions;
using Proton.Shared.Interfaces;

namespace Proton.Server.Resource;

public sealed class ServerResource : AsyncResource
{
    private readonly IHost host;

    public ServerResource()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        builder.ConfigureAppConfiguration(
            (builder) =>
            {
                builder.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);
            }
        );
        builder.ConfigureServices(
            (context, builder) =>
            {
                builder
                    .AddMemoryCache()
                    .AddSingleton<IHostLifetime, ResourceLifetime>()
                    .AddInfrastructure()
                    .AddAuthentication()
                    .AddRaceFeatures()
                    .AddCharacterCreator()
                    .AddIplFeatures();
            }
        );

        host = builder.Build();
    }

    public override void OnStart()
    {
        ResourceExtensions.RegisterMValueAdapters();

        for (var total = 1; total <= 20; ++total)
        {
            var prizePool = total * 200;
            Console.WriteLine($"Total players: {total}, prize pool: {prizePool}$");
            for (var finished = 1; finished <= total; ++finished)
            {
                Console.WriteLine($"\tFinish {finished}, receive {Calc(total, finished) / 100 * prizePool}$");
            }
            Console.WriteLine("=================");
        }

        // TODO: Add logging for startup
        _ = host.Services.GetServices<IStartup>();
        host.StartAsync().Wait();
    }

    public float Calc(int total, int finished)
    {
        var totalMin = Math.Min(total, 4);
        var splits = ((totalMin * (1 + totalMin)) >> 1) + Math.Max(total - 4, 0);
        return (100f / splits) * (MathF.Max(MathF.Min(total, 4) - finished, 0) + 1);
    }

    public override void OnStop()
    {
        host.StopAsync().Wait();
        host.Dispose();
    }

    public override IEntityFactory<IPlayer> GetPlayerFactory()
    {
        return new PPlayerFactory();
    }

    private sealed class ResourceLifetime : IHostLifetime
    {
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
