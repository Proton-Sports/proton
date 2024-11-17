using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared.Enums;
using Proton.Client.Resource.Features.Races.Abstractions;
using Proton.Shared.Constants;
using Proton.Shared.Models;

namespace Proton.Client.Resource.Features.Races;

public sealed class DefaultRaceService(IEnumerable<IRacePointResolver> resolvers) : IRaceService
{
    private const uint BlipSpriteObjective = 146;
    private const uint BlipSpriteArrow = 14;

    private readonly List<RacePointDto> racePoints = [];
    private readonly Dictionary<RaceType, IRacePointResolver> raceTypeToResolverDictionary = resolvers.ToDictionary(x =>
        x.SupportedRaceType
    );
    private readonly HashSet<Action<object>> hitEventHandlers = [];

    private bool hit;
    private int index;
    private ICheckpoint? checkpoint;
    private IBlip? blip;
    private IBlip? arrowBlip;
    private IMarker? nextMarker;
    private IBlip? nextBlip;

    public event Action? Started;
    public event Action? Stopped;

    public event Action<object> RacePointHit
    {
        add => hitEventHandlers.Add(value);
        remove => hitEventHandlers.Remove(value);
    }

    public int Dimension { get; set; }
    public IReadOnlyList<RacePointDto> RacePoints => racePoints;
    public RaceType RaceType { get; set; }
    public bool Ghosting { get; set; }
    public string? IplName { get; set; }
    public RaceStatus Status { get; set; }

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
        hit = false;
        this.index = index;
        var nextPoint = nextIndex is not null ? racePoints[(int)nextIndex] : default;
        var point = racePoints[index];
        if (checkpoint is null)
        {
            checkpoint = Alt.CreateCheckpoint(
                checkpointType,
                point.Position - new Position(0, 0, point.Radius / 2),
                nextPoint is null ? Position.Zero : nextPoint.Position - new Position(0, 0, nextPoint.Radius / 2),
                point.Radius,
                point.Radius,
                new Rgba(251, 251, 181, 128),
                new Rgba(0, 197, 252, 255),
                512
            );
            checkpoint.Dimension = Dimension;
        }
        else
        {
            checkpoint.CheckpointType = (byte)checkpointType;
            checkpoint.Position = point.Position - new Position(0, 0, point.Radius / 2);
            checkpoint.NextPosition = nextPoint is null
                ? Position.Zero
                : nextPoint.Position - new Position(0, 0, nextPoint.Radius / 2);
            checkpoint.Radius = point.Radius;
        }

        if (nextPoint is null)
        {
            if (nextMarker is not null)
            {
                nextMarker.Destroy();
                nextMarker = null;
            }
            if (nextBlip is not null)
            {
                nextBlip.Destroy();
                nextBlip = null;
            }
        }
        else
        {
            if (nextMarker is null)
            {
                nextMarker = Alt.CreateMarker(
                    MarkerType.MarkerCylinder,
                    nextPoint.Position,
                    new Rgba(251, 251, 181, 32),
                    true,
                    512
                );
                nextMarker.Dimension = Dimension;
            }
            else
            {
                nextMarker.Position = nextPoint.Position;
            }
            nextMarker.Scale = new Position(nextPoint.Radius * 2, nextPoint.Radius * 2, nextPoint.Radius);

            if (nextBlip is null)
            {
                nextBlip = Alt.CreatePointBlip(nextPoint.Position);
                nextBlip.Sprite = BlipSpriteObjective;
                nextBlip.Color = 5;
                nextBlip.ScaleXY = new Vector2(0.5f);
                nextBlip.Dimension = Dimension;
            }
            else
            {
                nextBlip.Position = nextPoint.Position;
            }
        }

        if (blip is null)
        {
            blip = Alt.CreatePointBlip(point.Position);
            blip.Sprite = BlipSpriteObjective;
            blip.ScaleXY = new Vector2(1f);
            blip.Color = 5;
            blip.Dimension = Dimension;
        }
        else
        {
            blip.Position = point.Position;
        }

        if (arrowBlip is null)
        {
            arrowBlip = Alt.CreatePointBlip(point.Position);
            arrowBlip.Sprite = BlipSpriteArrow;
            arrowBlip.ScaleXY = new Vector2(1f);
            arrowBlip.Color = 5;
            arrowBlip.Dimension = Dimension;
        }
        else
        {
            arrowBlip.Position = point.Position;
        }
        return checkpoint;
    }

    public void UnloadRacePoint()
    {
        if (checkpoint is not null)
        {
            checkpoint.Destroy();
            checkpoint = null;
        }
        if (blip is not null)
        {
            blip.Destroy();
            blip = null;
        }
        if (arrowBlip is not null)
        {
            arrowBlip.Destroy();
            arrowBlip = null;
        }
        if (nextMarker is not null)
        {
            nextMarker.Destroy();
            nextMarker = null;
        }
        if (nextBlip is not null)
        {
            nextBlip.Destroy();
            nextBlip = null;
        }
    }

    public void Start()
    {
        Status = RaceStatus.Started;
        Alt.OnTick += HandleTick;
        if (Started is not null)
        {
            Started();
        }
    }

    public void Stop()
    {
        Status = RaceStatus.None;
        Alt.OnTick -= HandleTick;
        if (Stopped is not null)
        {
            Stopped();
        }
    }

    public bool TryGetPointResolver(out IRacePointResolver resolver)
    {
        return raceTypeToResolverDictionary.TryGetValue(RaceType, out resolver!);
    }

    private void HandleTick()
    {
        if (hit || checkpoint is null || hitEventHandlers.Count == 0)
        {
            return;
        }

        var vehicle = Alt.LocalPlayer.Vehicle;
        if (vehicle is null)
        {
            return;
        }

        const float offset = 32f;
        var vehiclePosition = vehicle.Position;
        var radiusSquared = checkpoint.Radius * checkpoint.Radius;
        if (vehiclePosition.GetDistanceSquaredTo(checkpoint.Position) <= radiusSquared + offset)
        {
            hit = true;
            foreach (var handler in hitEventHandlers)
            {
                handler(index);
            }
        }
    }
}
