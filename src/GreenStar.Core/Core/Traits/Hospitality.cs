using System;
using System.Collections.Generic;
using GreenStar.Core.Persistence;

namespace GreenStar.Core.Traits
{
    public class Hospitality : Trait
    {
        private readonly Locatable _hostLocatable;

        public Hospitality(Locatable locatable)
        {
            _hostLocatable = locatable ?? throw new ArgumentNullException(nameof(locatable));
        }
        public override void Load(IPersistenceReader reader)
        {
            //TODO
        }

        public override void Persist(IPersistenceWriter writer)
        {
            //TODO
        }

        public List<Guid> ActorIds { get; set; } = new List<Guid>();

        public void Enter(Actor incomingActor)
        {
            var incomingLocation = incomingActor.Trait<Locatable>() ?? throw new InvalidOperationException("Cannot add a non locatable actor to a host.");

            incomingLocation.Position = _hostLocatable.Position;
            incomingLocation.HostLocationActorId = Self.Id;

            ActorIds.Add(incomingActor.Id);
        }

        public void Leave(Actor leavingActor)
        {
            var leavingLocation = leavingActor.Trait<Locatable>() ?? throw new InvalidOperationException("Cannot remove a non locatable actor to a host.");

            leavingLocation.Position = _hostLocatable.Position;
            leavingLocation.HostLocationActorId = Guid.Empty;

            ActorIds.Remove(leavingActor.Id);
        }
    }
}