using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared.Enums;
using Proton.Shared.Constants;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Races;

public sealed class DefaultRaceService : IRaceService
{
    private const uint BlipSpriteObjective = 146;
    private const uint BlipSpriteArrow = 14;

    private readonly List<RacePointDto> racePoints = new();
    private readonly Dictionary<int, Data> indexToDataDictionary = new();
    private readonly Dictionary<RaceType, IRacePointResolver> raceTypeToResolverDictionary;
    private readonly HashSet<Action<object>> hitEventHandlers = new();
    private bool started;

    public event Action<object> RacePointHit
    {
        add => hitEventHandlers.Add(value);
        remove => hitEventHandlers.Remove(value);
    }

    public int Dimension { get; set; }
    public int Laps { get; set; }
    public int CurrentLap { get; set; }
    public IReadOnlyList<RacePointDto> RacePoints => racePoints;
    public bool IsStarted => started;
    public RaceType RaceType { get; set; }

    public DefaultRaceService(IEnumerable<IRacePointResolver> resolvers)
    {
        raceTypeToResolverDictionary = resolvers.ToDictionary(x => x.SupportedRaceType);
    }

    public void ClearRacePoints()
    {
        racePoints.Clear();
    }

    public int EnsureRacePointsCapacity(int capacity)
    {
        return racePoints.EnsureCapacity(capacity);
    }

    public void AddRacePoint(RacePointDto point)
    {
        racePoints.Add(point);
    }

    public void AddRacePoints(List<RacePointDto> points)
    {
        racePoints.AddRange(points);
    }

    public ICheckpoint LoadRacePoint(CheckpointType checkpointType, int index, int? nextIndex)
    {
        var nextPoint = nextIndex is not null ? racePoints[(int)nextIndex] : default;
        var point = racePoints[index];
        var checkpoint = Alt.CreateCheckpoint(
            checkpointType,
            point.Position - new Position(0, 0, point.Radius / 2),
            nextPoint is null ? Position.Zero : nextPoint.Position - new Position(0, 0, nextPoint.Radius / 2),
            point.Radius,
            point.Radius,
            new Rgba(251, 251, 181, 128),
            new Rgba(0, 197, 252, 255),
            512);
        checkpoint.Dimension = Dimension;
        var blip = Alt.CreatePointBlip(point.Position);
        blip.Sprite = BlipSpriteObjective;
        blip.ScaleXY = new Vector2(1f);
        blip.Color = 5;
        blip.Dimension = Dimension;
        var arrowBlip = Alt.CreatePointBlip(point.Position);
        arrowBlip.Sprite = BlipSpriteArrow;
        arrowBlip.ScaleXY = new Vector2(1f);
        arrowBlip.Color = 5;
        arrowBlip.Dimension = Dimension;

        IMarker? nextMarker = default;
        IBlip? nextBlip = default;
        if (nextPoint is not null)
        {
            nextMarker = Alt.CreateMarker(MarkerType.MarkerCylinder, nextPoint.Position, new Rgba(251, 251, 181, 32), true, 512);
            nextMarker.Scale = new Position(nextPoint.Radius * 2, nextPoint.Radius * 2, nextPoint.Radius);
            nextMarker.Dimension = Dimension;
            nextBlip = Alt.CreatePointBlip(nextPoint.Position);
            nextBlip.Sprite = BlipSpriteObjective;
            nextBlip.Color = 5;
            nextBlip.ScaleXY = new Vector2(0.5f);
            nextBlip.Dimension = Dimension;
        }
        indexToDataDictionary[index] = new Data(checkpoint, blip, arrowBlip, nextMarker, nextBlip);
        return checkpoint;
    }

    public bool UnloadRacePoint(int index)
    {
        if (!indexToDataDictionary.TryGetValue(index, out var data)) return false;
        data.Destroy();
        indexToDataDictionary.Remove(index);
        return true;
    }

    public void Start()
    {
        started = true;
        Alt.OnTick += HandleTick;
    }

    public void Stop()
    {
        started = false;
        Alt.OnTick -= HandleTick;
    }

    public bool TryGetPointResolver(out IRacePointResolver resolver)
    {
        return raceTypeToResolverDictionary.TryGetValue(RaceType, out resolver!);
    }

    private void HandleTick()
    {
        var vehicle = Alt.LocalPlayer.Vehicle;
        if (vehicle is null || hitEventHandlers.Count == 0) return;

        const float offset = 32f;
        var vehiclePosition = vehicle.Position;
        foreach (var pair in indexToDataDictionary)
        {
            var checkpoint = pair.Value.Checkpoint;
            var radiusSquared = checkpoint.Radius * checkpoint.Radius;
            if (DistanceSquared(vehiclePosition, checkpoint.Position) <= radiusSquared + offset)
            {
                foreach (var handler in hitEventHandlers)
                {
                    handler(pair.Key);
                }
                break;
            }
        }
    }

    private static float DistanceSquared(Position a, Position b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    private class Data
    {
        public readonly ICheckpoint Checkpoint;
        public readonly IBlip Blip;
        public readonly IBlip ArrowBlip;
        public readonly IMarker? NextMarker;
        public readonly IBlip? NextBlip;

        public Data(ICheckpoint checkpoint, IBlip blip, IBlip arrowBlip, IMarker? nextMarker, IBlip? nextBlip)
        {
            Checkpoint = checkpoint;
            Blip = blip;
            ArrowBlip = arrowBlip;
            NextMarker = nextMarker;
            NextBlip = nextBlip;
        }

        public void Destroy()
        {
            Checkpoint.Destroy();
            Blip.Destroy();
            ArrowBlip.Destroy();
            NextMarker?.Destroy();
            NextBlip?.Destroy();
        }
    }
}
