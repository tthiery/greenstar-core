using System;
using System.Collections.Generic;

using GreenStar.Persistence;

namespace GreenStar.Traits;

public class Hospitality : Trait
{
    private readonly Locatable _hostLocatable;

    public List<Guid> ActorIds { get; } = new List<Guid>();

    public Hospitality(Locatable locatable)
    {
        _hostLocatable = locatable ?? throw new ArgumentNullException(nameof(locatable));
    }

    public override void Load(IPersistenceReader reader)
    {
        foreach (string property in reader.ReadPropertyNames(prefix: "ActorIds:"))
        {
            ActorIds.Add(reader.Read<Guid>(property));
        }
    }

    public override void Persist(IPersistenceWriter writer)
    {
        for (int idx = 0; idx < ActorIds.Count; idx++)
        {
            writer.Write("ActorIds:" + idx, ActorIds[idx]);
        }
    }

    public void Enter(Actor incomingActor)
    {
        var incomingLocation = incomingActor.Trait<Locatable>() ?? throw new InvalidOperationException("Cannot add a non locatable actor to a host.");

        incomingLocation.SetPosition(Self.Id);

        ActorIds.Add(incomingActor.Id);
    }

    public void Leave(IActorContext actorContext, Actor leavingActor)
    {
        var leavingLocation = leavingActor.Trait<Locatable>() ?? throw new InvalidOperationException("Cannot remove a non locatable actor to a host.");

        leavingLocation.SetPosition(_hostLocatable.GetPosition(actorContext));

        ActorIds.Remove(leavingActor.Id);
    }
}
