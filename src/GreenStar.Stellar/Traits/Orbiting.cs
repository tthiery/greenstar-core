using System;

using GreenStar.Cartography;

namespace GreenStar.Traits;

public class Orbiting : StellarMoving
{
    private readonly Locatable _orbitingLocatable;

    public Guid Host { get; set; }

    public double Distance { get; set; }
    public double SpeedDegree { get; set; }
    public double CurrentDegree { get; set; }

    public Orbiting(Locatable orbitingLocatable)
    {
        _orbitingLocatable = orbitingLocatable;
    }
    public override void Load(Persistence.IPersistenceReader reader)
    {
        Host = reader.Read<Guid>(nameof(Host));
        Distance = reader.Read<double>(nameof(Distance));
        SpeedDegree = reader.Read<double>(nameof(SpeedDegree));
        CurrentDegree = reader.Read<double>(nameof(CurrentDegree));
    }

    public override void Persist(Persistence.IPersistenceWriter writer)
    {
        writer.Write(nameof(Host), Host);
        writer.Write(nameof(Distance), Distance);
        writer.Write(nameof(SpeedDegree), SpeedDegree);
        writer.Write(nameof(CurrentDegree), CurrentDegree);
    }

    public override void Move(Context context)
        => Move(context.ActorContext);

    public void Move(IActorContext actorContext)
    {
        if (Host != Guid.Empty)
        {
            var host = actorContext.GetActor(Host);

            Move(actorContext, host);
        }
    }

    public void Move(IActorContext actorContext, Actor? host)
    {
        var hostLocatable = host?.Trait<Locatable>()?.GetPosition(actorContext) ?? throw new InvalidOperationException("Something orbiting needs some position to orbit around");

        var newDegree = CurrentDegree + SpeedDegree;
        CurrentDegree = (short)(newDegree % 360);

        _orbitingLocatable.SetPosition(new Coordinate(
            (long)(hostLocatable.X + Distance * Math.Cos(CurrentDegree * Math.PI / 180)),
            (long)(hostLocatable.Y + Distance * Math.Sin(CurrentDegree * Math.PI / 180))
        ));
    }
}
