using System;

using GreenStar.Cartography;
using GreenStar.Persistence;
using GreenStar.TurnEngine;

namespace GreenStar.Traits;

public class Locatable : Trait, IMaterialize
{
    // Located by another locatable actor
    public Guid HostLocationActorId { get; private set; } = Guid.Empty;

    // Located by independent position
    public Coordinate Position { get; private set; } = Coordinate.Zero;

    public override void Load(IPersistenceReader reader)
    {
        HostLocationActorId = reader.Read<Guid>(nameof(HostLocationActorId));
        Position = reader.Read<string>(nameof(Position));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write(nameof(HostLocationActorId), HostLocationActorId);
        writer.Write<string>(nameof(Position), Position.ToString());
    }

    public bool HasOwnPosition
        => HostLocationActorId == Guid.Empty;

    public void SetPosition(Guid host)
    {
        HostLocationActorId = host;
        Position = Coordinate.Zero;
    }
    public void SetPosition(Coordinate coordinate)
    {
        HostLocationActorId = Guid.Empty;
        Position = coordinate;
    }

    public Actor? GetHostLocationActor(Context context)
        => (HostLocationActorId != Guid.Empty) ? context.ActorContext.GetActor(HostLocationActorId) : null;

    public Coordinate GetPosition(IActorView actorView)
    {
        if (HasOwnPosition)
        {
            return Position;
        }
        else
        {
            var hostActor = actorView.GetActor(HostLocationActorId) ?? throw new InvalidOperationException("queries host actor but was not found");

            return hostActor.Trait<Locatable>().GetPosition(actorView);
        }
    }

    public void Materialize(TurnManager turnManager)
    {
        CurrentPosition = GetPosition(turnManager.Actors);
    }

    // [Exposed(DiscoveryLevel.LocationAware)]    
    public Coordinate CurrentPosition { get; set; } = Coordinate.Zero;
}
