using System;
using GreenStar.Core.Cartography;
using GreenStar.Core.Persistence;

namespace GreenStar.Core.Traits
{
    public class Locatable : Trait
    {
        public Coordinate Position { get; set; } = Coordinate.Zero;

        public Guid HostLocationActorId { get; set; } = Guid.Empty;

        public override void Load(IPersistenceReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            HostLocationActorId = reader.Read<Guid>(nameof(HostLocationActorId));
            Position = reader.Read<string>(nameof(Position));
        }

        public override void Persist(IPersistenceWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(nameof(HostLocationActorId), HostLocationActorId);
            writer.Write<string>(nameof(Position), Position.ToString());
        }

        public bool HasOwnPosition
            => HostLocationActorId == Guid.Empty;

        public Actor? GetHostLocationActor(Context context)
            => (HostLocationActorId != Guid.Empty) ? context.ActorContext.GetActor(HostLocationActorId) : null;


        public Coordinate CalculatePosition(IActorContext actorContext)
        {
            if (HasOwnPosition)
            {
                return Position;
            }
            else
            {
                var hostActor = actorContext.GetActor(HostLocationActorId) ?? throw new InvalidOperationException("queries host actor but was not found");

                return hostActor.Trait<Locatable>().CalculatePosition(actorContext);
            }
        }
    }
}