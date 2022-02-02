using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar.Core;

public interface IActorView
{
    Actor? GetActor(Guid actorId);
    IQueryable<Actor> AsQueryable();
    IEnumerable<TActor> GetActors<TActor, TTrait>(Func<TTrait, bool>? predicate = null)
        where TActor : Actor
        where TTrait : Trait;
}
public interface IActorContext
{
    void AddActor(Actor actor);
    void RemoveActor(Actor actor);
    Actor? GetActor(Guid actorId);
    IQueryable<Actor> AsQueryable();
    IEnumerable<TActor> GetActors<TActor, TTrait>(Func<TTrait, bool>? predicate = null)
        where TActor : Actor
        where TTrait : Trait;
    Actor? GetRandomActor(Func<Actor, bool> predicate);
}
