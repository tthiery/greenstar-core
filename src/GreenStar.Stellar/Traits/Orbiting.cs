using System;

using GreenStar.Cartography;

namespace GreenStar.Traits;

public class Orbiting : StellarMoving
{
    private readonly Locatable _itemInOrbit;

    public Guid Host { get; set; }

    public double Distance { get; set; }
    public double SpeedDegree { get; set; }
    public double CurrentDegree { get; set; }

    public Orbiting(Locatable itemInOrbit)
    {
        _itemInOrbit = itemInOrbit;
    }
    public override void Load(Persistence.IPersistenceReader reader)
    {
        base.Load(reader);

        Host = reader.Read<Guid>(nameof(Host));
        Distance = reader.Read<long>(nameof(Distance));
        SpeedDegree = reader.Read<int>(nameof(SpeedDegree));
        CurrentDegree = reader.Read<short>(nameof(CurrentDegree));
    }

    public override void Persist(Persistence.IPersistenceWriter writer)
    {
        base.Persist(writer);

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

            Move(host);
        }
    }

    public void Move(Actor? host)
    {
        var hostLocatable = host?.Trait<Locatable>()?.Position ?? throw new InvalidOperationException("Something orbiting needs some position to orbit around");

        var newDegree = CurrentDegree + SpeedDegree;
        CurrentDegree = (short)(newDegree % 360);

        _itemInOrbit.Position = new Coordinate(
            (long)(hostLocatable.X + Distance * Math.Cos(CurrentDegree * Math.PI / 180)),
            (long)(hostLocatable.Y + Distance * Math.Sin(CurrentDegree * Math.PI / 180))
        );
    }
}
