using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Traits;

namespace GreenStar;

public class PlayerActorView : IActorView
{
    private readonly IActorView _source;
    private readonly Guid _playerId;

    public PlayerActorView(IActorView source, Guid playerId)
    {
        _source = source;
        _playerId = playerId;
    }
    public IQueryable<Actor> AsQueryable()
        => _source.AsQueryable().Where(a => a.IsKnownTo(_playerId));

    public Actor? GetActor(Guid actorId)
    {
        var actor = _source.GetActor(actorId);

        return actor.IsKnownTo(_playerId) ? actor : null;
    }

    public TActor? GetActor<TActor>(Guid actorId) where TActor : Actor
    {
        var actor = _source.GetActor<TActor>(actorId);

        return actor.IsKnownTo(_playerId) ? actor : null;
    }

    public IEnumerable<TActor> GetActors<TActor, TTrait>(Func<TTrait, bool>? predicate = null)
        where TActor : Actor
        where TTrait : Trait
        => _source.GetActors<TActor, TTrait>(predicate).Where(t => t.IsKnownTo(_playerId));
}

file static class DiscoverableHelper
{
    extension(Actor? actor)
    {
        public bool IsKnownTo(Guid playerId)
        {
            if (actor is null)
            {
                return false;
            }
            if (actor?.TryGetTrait<Discoverable>(out var discoverable) ?? false)
            {
                return (discoverable.IsDiscoveredBy(playerId, DiscoveryLevel.Known));
            }
            // element does not participate in discoverability
            else
            {
                return true;
            }
        }
    }
}