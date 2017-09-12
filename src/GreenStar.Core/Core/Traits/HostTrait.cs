using System;
using System.Collections.Generic;
using GreenStar.Core.Persistence;

namespace GreenStar.Core.Traits
{
    public class HostTrait : Trait
    {
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
            var hostLocation = Self.Trait<LocatableTrait>() ?? throw new InvalidOperationException("Host has to have a location");
            var incomingLocation = incomingActor.Trait<LocatableTrait>() ?? throw new InvalidOperationException("Cannot add a non locatable actor to a host.");

            incomingLocation.Position = hostLocation.Position;
            incomingLocation.HostLocationActorId = Self.Id;

            ActorIds.Add(incomingActor.Id);
        }

        public void Leave(Actor leavingActor)
        {
            var hostLocation = Self.Trait<LocatableTrait>() ?? throw new InvalidOperationException("Host has to have a location");
            var leavingLocation = leavingActor.Trait<LocatableTrait>() ?? throw new InvalidOperationException("Cannot remove a non locatable actor to a host.");

            leavingLocation.Position = hostLocation.Position;
            leavingLocation.HostLocationActorId = Guid.Empty;

            ActorIds.Remove(leavingActor.Id);
        }
    }
}