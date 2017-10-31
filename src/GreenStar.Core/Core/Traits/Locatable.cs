using System;
using GreenStar.Core.Cartography;
using GreenStar.Core.Persistence;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core.Traits
{
    public class Locatable : Trait
    {
        public override void Load(IPersistenceReader reader)
        {
            //TODO
        }

        public override void Persist(IPersistenceWriter writer)
        {
            //TODO
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