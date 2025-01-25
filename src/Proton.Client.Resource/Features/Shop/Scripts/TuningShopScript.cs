using AltV.Net.Client;
using AltV.Net.Client.Elements.Data;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Shared.Enums;
using Proton.Client.Resource.Commons;
using Proton.Client.Resource.Features.UiViews.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Dtos;

namespace Proton.Client.Resource.Features.Shop.Scripts;

public sealed class TuningShopScript(IUiView ui) : HostedService
{
    readonly List<Position> markerPositions =
        new()
        {
            new(451.07782f, 5611.5645f, -80.02029f),
            new(461.65985f, 5611.215f, -80.02029f),
            new(471.04004f, 5611.1133f, -80.020294f)
        };

    public override Task StartAsync(CancellationToken ct)
    {
        foreach (var pos in markerPositions)
        {
            Alt.CreateMarker(MarkerType.MarkerMoney, pos, new Rgba(255, 0, 0, 255), true, 128);
        }
        Alt.OnServer<TuningShopMountDto>("tuning-shop.mount", OnServerMount);
        Alt.OnConsoleCommand += (cmd, args) =>
        {
            if (cmd.Equals("t"))
            {
                Alt.EmitServer("tuning-shop.mount");
            }
            else if (cmd.Equals("pos"))
            {
                Console.WriteLine($"{Alt.LocalPlayer.Position}");
                Console.WriteLine(
                    $"{Alt.LocalPlayer.Position.X}, {Alt.LocalPlayer.Position.Y}, {Alt.LocalPlayer.Position.Z}"
                );
            }
            else if (cmd.Equals("spawn") && long.TryParse(args[0], out var id))
            {
                Alt.EmitServer("tuning-shop.dev.spawn", id);
            }
            else if (cmd.Equals("generate"))
            {
                if (Alt.LocalPlayer.Vehicle is not IVehicle vehicle)
                {
                    return;
                }

                Alt.EmitServer(
                    "tuning-shop.dev.generate",
                    new TuningShopGenerateDto
                    {
                        Wheels =
                        [
                            .. Enum.GetValues<WheelType>()
                                .Where(a => IsWheelTypeAllowed(vehicle, a))
                                .Select(a =>
                                {
                                    Alt.Natives.SetVehicleWheelType(vehicle, (int)a);
                                    return new TuningShopGenerateWheelDto
                                    {
                                        Type = a,
                                        Count = Alt.Natives.GetNumVehicleMods(vehicle, 23)
                                    };
                                })
                        ]
                    }
                );
            }
        };
        Alt.OnKeyUp += GlobalOnKeyUp;
        ui.On<int, int>("tuning-shop.values.change", OnUiValueChange);
        ui.On<int, string>("tuning-shop.colors.change", OnUiColorsChange);
        ui.On<int, long>("tuning-shop.buy", OnUiBuy);
        ui.On<int, string>("tuning-shop.colors.buy", OnUiBuyColor);
        ui.On<int, long, bool>("tuning-shop.toggle", OnUiToggle);
        ui.On<int, int>("tuning-shop.wheels.change", OnUiWheelsChange);
        ui.On<long>("tuning-shop.wheels.buy", OnUiWheelsBuy);
        ui.On<TuningShopWheelVariationDto, bool>("tuning-shop.wheels.toggle", OnUiWheelsToggle);
        Alt.OnServer<int, string, bool>("tuning-shop.colors.buy", OnServerBuyColor);
        Alt.OnServer<int, long, bool>("tuning-shop.buy", OnServerBuy);
        Alt.OnServer<long, bool>("tuning-shop.wheels.buy", OnServerWheelsBuy);
        ui.OnMount(Route.TuningShop, OnUiMount);
        ui.OnUnmount(Route.TuningShop, OnUiUnmount);
        return Task.CompletedTask;
    }

    void OnServerMount(TuningShopMountDto dto)
    {
        if (Alt.LocalPlayer.Vehicle is not IVehicle vehicle)
        {
            return;
        }

        foreach (var type in Enum.GetValues<WheelType>())
        {
            if (IsWheelTypeAllowed(vehicle, type))
            {
                dto.WheelType = type;
                break;
            }
        }
        ui.Mount(Route.TuningShop, dto);
    }

    void OnUiMount()
    {
        Alt.OnKeyUp += OnKeyUp;
    }

    void OnUiUnmount()
    {
        Alt.OnKeyUp -= OnKeyUp;
        Alt.EmitServer("tuning-shop.unmount");
    }

    void OnKeyUp(Key key)
    {
        switch (key)
        {
            case Key.Escape:
                if (!Alt.GameControlsEnabled)
                {
                    Alt.GameControlsEnabled = true;
                }
                ui.Unmount(Route.TuningShop);
                break;
            case Key.Menu:
                Alt.GameControlsEnabled = !Alt.IsCursorVisible;
                break;
        }
    }

    void GlobalOnKeyUp(Key key)
    {
        if (key != Key.E || ui.IsMounted(Route.TuningShop))
        {
            return;
        }

        var playerPos = Alt.LocalPlayer.Position;
        foreach (var pos in markerPositions)
        {
            if (playerPos.GetDistanceSquaredTo(pos) < 16)
            {
                Alt.EmitServer("tuning-shop.mount");
                break;
            }
        }
    }

    void OnUiValueChange(int category, int value)
    {
        var vehicle = Alt.LocalPlayer.Vehicle;
        if (vehicle is null)
        {
            return;
        }

        Alt.EmitServer("tuning-shop.values.change", category, value);
        // Alt.Natives.SetVehicleModKit(vehicle, 0);
        // Alt.Natives.SetVehicleMod(vehicle, category, value, false);
    }

    void OnUiColorsChange(int category, string hex)
    {
        var vehicle = Alt.LocalPlayer.Vehicle;
        if (vehicle is null)
        {
            return;
        }
        Alt.EmitServer("tuning-shop.colors.change", category, hex);
        // #pragma warning disable CA1846 // Prefer 'AsSpan' over 'Substring'
        //         if (
        //             !int.TryParse(hex.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)
        //             || !int.TryParse(hex.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g)
        //             || !int.TryParse(hex.Substring(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b)
        //         )
        //         {
        //             return;
        //         }
        // #pragma warning restore CA1846 // Prefer 'AsSpan' over 'Substring'

        //         switch (category)
        //         {
        //             case 66: // primary
        //                 Alt.Natives.SetVehicleCustomPrimaryColour(vehicle, r, g, b);
        //                 break;
        //             case 67: // secondary
        //                 Alt.Natives.SetVehicleCustomSecondaryColour(vehicle, r, g, b);
        //                 break;
        //             default:
        //                 break;
        //         }
    }

    void OnUiBuy(int category, long modId)
    {
        Alt.EmitServer("tuning-shop.buy", category, modId);
    }

    void OnServerBuy(int category, long modId, bool success)
    {
        ui.Emit("tuning-shop.buy", category, modId, success);
    }

    void OnUiBuyColor(int category, string hex)
    {
        if (category != 66 && category != 67)
        {
            return;
        }
        Alt.EmitServer("tuning-shop.colors.buy", category, hex);
    }

    void OnServerBuyColor(int category, string hex, bool success)
    {
        ui.Emit("tuning-shop.colors.buy", category, hex, success);
    }

    void OnUiToggle(int category, long modId, bool value)
    {
        Alt.EmitServer("tuning-shop.toggle", category, modId, value);
    }

    void OnUiWheelsChange(int typeInt, int value)
    {
        var type = (WheelType)typeInt;
        if (!Enum.IsDefined(type) || !Enum.GetValues<WheelType>().Contains(type))
        {
            return;
        }

        Alt.EmitServer("tuning-shop.wheels.change", typeInt, value);
    }

    void OnUiWheelsBuy(long wheelVariationId)
    {
        Alt.EmitServer("tuning-shop.wheels.buy", wheelVariationId);
    }

    void OnServerWheelsBuy(long wheelVariationId, bool success)
    {
        ui.Emit("tuning-shop.wheels.buy", wheelVariationId, success);
    }

    void OnUiWheelsToggle(TuningShopWheelVariationDto dto, bool value)
    {
        Alt.EmitServer("tuning-shop.wheels.toggle", dto, value);
    }

    static bool IsWheelTypeAllowed(IVehicle vehicle, WheelType type)
    {
        return (VehicleClass)Alt.Natives.GetVehicleClass(vehicle) switch
        {
            VehicleClass.Cycles => false,
            VehicleClass.Motorcycles => type == WheelType.Bike,
            VehicleClass.OpenWheels => type == WheelType.OpenWheel,
            _ => true,
        };
    }
}
