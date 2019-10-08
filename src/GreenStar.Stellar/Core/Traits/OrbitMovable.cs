using System;
using GreenStar.Algorithms;
using GreenStar.Core.Cartography;

namespace GreenStar.Core.Traits
{
    public class OrbitMovable : Trait
    {
        private readonly Locatable _objectInOrbit;

        public bool IsBoundToHost { get; private set; }
        public Guid Host { get; set; }

        public long Distance { get; set; }
        public int SpeedDegree { get; set; }
        public short CurrentDegree { get; set; }

        public OrbitMovable(Locatable objectInOrbit, bool isBoundToHost)
        {
            _objectInOrbit = objectInOrbit;

            IsBoundToHost = isBoundToHost;
        }
        public override void Load(Persistence.IPersistenceReader reader)
        {
            base.Load(reader);

            IsBoundToHost = reader.Read<bool>(nameof(IsBoundToHost));
            Host = reader.Read<Guid>(nameof(Host));
            Distance = reader.Read<long>(nameof(Distance));
            CurrentDegree = reader.Read<short>(nameof(CurrentDegree));
        }

        public override void Persist(Persistence.IPersistenceWriter writer)
        {
            base.Persist(writer);

            writer.Write(nameof(IsBoundToHost), IsBoundToHost);
            writer.Write(nameof(Host), Host);
            writer.Write(nameof(Distance), Distance);
            writer.Write(nameof(CurrentDegree), CurrentDegree);
        }

        public void Move(Context context)
        {
            var host = context.ActorContext.GetActor(Host);

            var hostLocatable = host.Trait<Locatable>().Position;

            var newDegree = CurrentDegree + SpeedDegree;
            CurrentDegree = (short)(newDegree % 360);

            _objectInOrbit.Position = new Coordinate(
                (long)(hostLocatable.X + Distance * Math.Cos(CurrentDegree)),
                (long)(hostLocatable.Y + Distance * Math.Sin(CurrentDegree))
            );
        }
    }
}