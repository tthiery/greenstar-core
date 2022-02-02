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
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        foreach (string property in reader.ReadPropertyNames(prefix: "ActorIds:"))
        {
            ActorIds.Add(reader.Read<Guid>(property));
        }
    }

    public override void Persist(IPersistenceWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        for (int idx = 0; idx < ActorIds.Count; idx++)
        {
            writer.Write("ActorIds:" + idx, ActorIds[idx]);
        }
    }

    public void Enter(Actor incomingActor)
    {
        if (incomingActor == null)
        {
            throw new ArgumentNullException(nameof(incomingActor));
        }

        var incomingLocation = incomingActor.Trait<Locatable>() ?? throw new InvalidOperationException("Cannot add a non locatable actor to a host.");

        incomingLocation.Position = _hostLocatable.Position;
        incomingLocation.HostLocationActorId = Self.Id;

        ActorIds.Add(incomingActor.Id);
    }

    public void Leave(Actor leavingActor)
    {
        if (leavingActor == null)
        {
            throw new ArgumentNullException(nameof(leavingActor));
        }

        var leavingLocation = leavingActor.Trait<Locatable>() ?? throw new InvalidOperationException("Cannot remove a non locatable actor to a host.");

        leavingLocation.Position = _hostLocatable.Position;
        leavingLocation.HostLocationActorId = Guid.Empty;

        ActorIds.Remove(leavingActor.Id);
    }
}
