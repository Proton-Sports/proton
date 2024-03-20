using System.Numerics;
using AltV.Net.Client;
using AltV.Net.Client.Elements.Interfaces;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Shared.Enums;
using Proton.Client.Core.Interfaces;
using Proton.Shared.Models;

namespace Proton.Client.Infrastructure.Services;

public class LandRaceCreator : IRaceCreator
{
    private readonly LinkedList<RacePointData> racePointDatas = new();
    private readonly LinkedList<StartPositionData> startPointDatas = new();
    private const uint BlipSpriteRadarRaceLand = 315;
    private const uint BlipSpriteRadarPlaceholder6 = 373;
    private const uint RadarRaceOpenWheel = 726;
    private const uint BlipColorPrimary = 1;
    private const uint BlipColorSecondary = 4;

    public string Name { get; set; } = string.Empty;
    public IEnumerable<SharedRacePoint> RacePoints => racePointDatas.Select(x => new SharedRacePoint(x.Position, x.Checkpoint.Radius));
    public IEnumerable<SharedRaceStartPoint> StartPoints => startPointDatas.Select(x => new SharedRaceStartPoint(x.Position, x.Rotation));

    public void ClearRacePoints()
    {
        foreach (var data in racePointDatas)
        {
            data.Destroy();
        }
        racePointDatas.Clear();
    }

    public void ClearStartPoints()
    {
        foreach (var data in startPointDatas)
        {
            data.Destroy();
        }
        startPointDatas.Clear();
    }

    public void AddRacePoint(Position position, float radius)
    {
        var lastData = racePointDatas.Last?.Value;
        if (lastData is null)
        {
            racePointDatas.AddLast(CreateRacePositionData(CheckpointType.CylinderCheckerboard, BlipSpriteRadarRaceLand, position, position, radius));
            return;
        }

        lastData.Checkpoint.NextPosition = position;
        lastData.Checkpoint.CheckpointType = (byte)CheckpointType.CylinderDoubleArrow;
        lastData.Blip.Sprite = BlipSpriteRadarPlaceholder6;
        lastData.Blip.Color = BlipColorSecondary;

        racePointDatas.AddLast(CreateRacePositionData(CheckpointType.CylinderCheckerboard, BlipSpriteRadarRaceLand, position, position, radius));
    }

    public void RemoveRacePoint(Position position)
    {
        var closestDistance = float.MaxValue;
        LinkedListNode<RacePointData>? closestNode = default;
        for (var node = racePointDatas.First; node is not null; node = node.Next)
        {
            var dist = position.GetDistanceSquaredTo(node.Value.Position);
            if (dist <= node.Value.Checkpoint.Radius && dist < closestDistance)
            {
                closestDistance = dist;
                closestNode = node;
            }
        }
        if (closestNode is null) return;

        closestNode.Value.Destroy();
        if (closestNode.Previous is not null)
        {
            if (closestNode.Next is not null)
            {
                closestNode.Previous.Value.Checkpoint.NextPosition = closestNode.Next.Value.Checkpoint.Position;
            }
            else
            {
                var data = closestNode.Previous.Value;
                data.Checkpoint.CheckpointType = (byte)CheckpointType.CylinderCheckerboard;
                data.Blip.Sprite = BlipSpriteRadarRaceLand;
                data.Blip.Color = BlipColorPrimary;
            }
        }
        racePointDatas.Remove(closestNode);
    }

    public bool TryGetClosestRaceCheckpointTo(Position position, out ICheckpoint checkpoint)
    {
        var precision = 0.5f;
        checkpoint = default!;
        float maxSquared = float.MaxValue;
        foreach (var data in racePointDatas)
        {
            var distanceSquared = Vector3.DistanceSquared(position, data.Checkpoint.Position) - precision;
            if (distanceSquared < (data.Checkpoint.Radius * data.Checkpoint.Radius) && distanceSquared < maxSquared)
            {
                checkpoint = data.Checkpoint;
                maxSquared = distanceSquared;
            }
        }
        return checkpoint != default;
    }

    public bool UpdateRacePointPosition(ICheckpoint checkpoint, Position position)
    {
        for (var node = racePointDatas.First; node is not null; node = node.Next)
        {
            var current = node.Value;
            if (current.Checkpoint != checkpoint) continue;

            current.Checkpoint.Position = position;
            current.Blip.Position = position;
            var previous = node.Previous?.Value;
            if (previous is not null)
            {
                previous.Checkpoint.NextPosition = position;
            }
            return true;
        }
        return false;
    }

    public void AddStartPoint(Position position, Rotation rotation)
    {
        startPointDatas.AddLast(CreateStartPositionData(startPointDatas.Count, position, rotation));
    }

    public void RemoveStartPoint(Position position)
    {
        var closestDistance = float.MaxValue;
        LinkedListNode<StartPositionData>? closestNode = default;
        for (var node = startPointDatas.First; node is not null; node = node.Next)
        {
            var dist = position.GetDistanceSquaredTo(node.Value.Position);
            if (dist < 4f && dist < closestDistance)
            {
                closestDistance = dist;
                closestNode = node;
            }
        }
        if (closestNode is null) return;

        MarkerType previousMarkerType = closestNode.Value.NumberMarker.MarkerType;
        for (var node = closestNode.Next; node is not null; node = node.Next)
        {
            (previousMarkerType, node.Value.NumberMarker.MarkerType) = (node.Value.NumberMarker.MarkerType, previousMarkerType);
        }
        closestNode.Value.Destroy();
        startPointDatas.Remove(closestNode);
    }

    private RacePointData CreateRacePositionData(CheckpointType checkpointType, uint blipSprite, Position position, Position nextPosition, float radius)
    {
        var blip = Alt.CreatePointBlip(position);
        blip.Sprite = blipSprite;
        blip.Color = BlipColorPrimary;

        return new RacePointData(
            position,
            Alt.CreateCheckpoint(checkpointType, position, nextPosition, racePointDatas.Last?.Value.Checkpoint.Radius ?? radius, 6f, new Rgba(255, 255, 255, 255), new Rgba(255, 0, 0, 255), 256),
            blip);
    }

    private static StartPositionData CreateStartPositionData(int ordinal, Position position, Rotation rotation)
    {
        var blip = Alt.CreatePointBlip(position);
        blip.Sprite = RadarRaceOpenWheel;
        var numberMarker = Alt.CreateMarker(MarkerType.MarkerNum0 + ordinal, position + new Position(0, 0, 2), new Rgba(255, 0, 0, 255), true, 64);
        numberMarker.IsFaceCamera = true;

        var boxMarker = Alt.CreateMarker(MarkerType.MarkerBoxes, position, new Rgba(255, 0, 0, 64), true, 64);
        boxMarker.Rotation = rotation;
        boxMarker.Scale = new Position(3, 6, 3);
        return new StartPositionData(
            position,
            rotation,
            numberMarker,
            boxMarker,
            blip);
    }

    public void ImportStartPoints(IEnumerable<SharedRaceStartPoint> points)
    {
        foreach (var point in points)
        {
            AddStartPoint(point.Position, point.Rotation);
        }
    }

    public void ImportRacePoints(IEnumerable<SharedRacePoint> points)
    {
        foreach (var point in points)
        {
            AddRacePoint(point.Position, point.Radius);
        }
    }

    private abstract class BasePositionData
    {
        public IBlip Blip { get; set; }
        public Position Position { get; set; }

        public BasePositionData(Position position, IBlip blip)
        {
            Position = position;
            Blip = blip;
        }

        public virtual void Destroy()
        {
            Blip.Destroy();
        }
    }

    private class RacePointData : BasePositionData
    {
        public ICheckpoint Checkpoint { get; set; }

        public RacePointData(Position position, ICheckpoint checkpoint, IBlip blip) : base(position, blip)
        {
            Checkpoint = checkpoint;
        }

        public override void Destroy()
        {
            base.Destroy();
            Checkpoint.Destroy();
        }
    }

    private class StartPositionData : BasePositionData
    {
        public IMarker NumberMarker { get; set; }
        public IMarker BoxMarker { get; set; }
        public Rotation Rotation { get; set; }

        public StartPositionData(Position position, Rotation rotation, IMarker numberMarker, IMarker boxMarker, IBlip blip) : base(position, blip)
        {
            Rotation = rotation;
            NumberMarker = numberMarker;
            BoxMarker = boxMarker;
        }

        public override void Destroy()
        {
            base.Destroy();
            NumberMarker.Destroy();
            BoxMarker.Destroy();
        }
    }
}
