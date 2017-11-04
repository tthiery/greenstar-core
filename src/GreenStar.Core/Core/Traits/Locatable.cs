using System;
using GreenStar.Core.Cartography;
using GreenStar.Core.Persistence;

namespace GreenStar.Core.Traits
{
    public class Locatable : Trait
    {
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

        public Coordinate Position { get; set; }

        public Guid HostLocationActorId { get; set; } = Guid.Empty;

        public bool HasOwnPosition
            => HostLocationActorId == Guid.Empty;

        public Coordinate CalculatePosition(IActorContext actorContext)
        {
            if (HasOwnPosition)
            {
                return Position;
            }
            else
            {
                var hostActor = actorContext.GetActor(HostLocationActorId);

                return hostActor.Trait<Locatable>().CalculatePosition(actorContext);
            }
        }
    }
}