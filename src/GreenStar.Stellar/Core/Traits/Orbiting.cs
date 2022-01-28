using System;
using GreenStar.Core.Cartography;

namespace GreenStar.Core.Traits
{

    public class Orbiting : StellarMoving
    {
        private readonly Locatable _objectInOrbit;

        public Guid Host { get; set; }

        public long Distance { get; set; }
        public int SpeedDegree { get; set; }
        public short CurrentDegree { get; set; }

        public Orbiting(Locatable objectInOrbit)
        {
            _objectInOrbit = objectInOrbit;
        }
        public override void Load(Persistence.IPersistenceReader reader)
        {
            base.Load(reader);

            Host = reader.Read<Guid>(nameof(Host));
            Distance = reader.Read<long>(nameof(Distance));
            CurrentDegree = reader.Read<short>(nameof(CurrentDegree));
        }

        public override void Persist(Persistence.IPersistenceWriter writer)
        {
            base.Persist(writer);

            writer.Write(nameof(Host), Host);
            writer.Write(nameof(Distance), Distance);
            writer.Write(nameof(CurrentDegree), CurrentDegree);
        }

        public override void Move(Context context)
        {
            if (Host != Guid.Empty)
            {
                var host = context.ActorContext.GetActor(Host);

                var hostLocatable = host?.Trait<Locatable>()?.Position ?? throw new InvalidOperationException("Something orbiting needs some position to orbit around");

                var newDegree = CurrentDegree + SpeedDegree;
                CurrentDegree = (short)(newDegree % 360);

                _objectInOrbit.Position = new Coordinate(
                    (long)(hostLocatable.X + Distance * Math.Cos(CurrentDegree * Math.PI / 180)),
                    (long)(hostLocatable.Y + Distance * Math.Sin(CurrentDegree * Math.PI / 180))
                );
            }
        }
    }
}