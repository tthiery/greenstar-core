using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using GreenStar;

namespace GreenStar.TurnEngine;

public class InMemoryActorStore : IActorContext, IActorView
{
    private ImmutableList<Actor> _actors;
    public InMemoryActorStore(Guid id, IEnumerable<Actor> actors)
    {
        Id = id;
        _actors = actors?.ToImmutableList() ?? throw new ArgumentNullException(nameof(actors));
    }

    public Guid Id { get; }
    public IEnumerable<Actor> Actors { get => _actors; }

    public Actor? GetActor(Guid actorId)
        => Actors.FirstOrDefault(a => a.Id == actorId);
    public TActor? GetActor<TActor>(Guid actorId)
        where TActor : Actor
        => this.GetActor(actorId) as TActor;

    public void AddActor(Actor actor)
        => _actors = _actors.Add(actor);

    public void RemoveActor(Actor actor)
        => _actors = _actors.Remove(actor);

    public IQueryable<Actor> AsQueryable()
        => _actors.AsQueryable();



    public Actor? GetRandomActor(Func<Actor, bool> predicate)
        => _actors.Where(predicate).AsQueryable().TakeOneRandom();

    public IEnumerable<TActor> GetActors<TActor, TTrait>(Func<TTrait, bool>? predicate = null)
        where TActor : Actor
        where TTrait : Trait
        => AsQueryable()
            .Where(a =>
                a is TActor
                && a.HasTrait<TTrait>()
                && (predicate == null || predicate(a.Trait<TTrait>())))
            .Cast<TActor>();
}