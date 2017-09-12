using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GreenStar.Core.TurnEngine
{
    public class Game
    {
        private ImmutableList<Actor> _actors;
        public Game(Guid id, IEnumerable<Player> players, IEnumerable<Actor> actors)
        {
            Id = id;
            Players = players ?? throw new ArgumentNullException(nameof(players));
            _actors = actors?.ToImmutableList() ?? throw new ArgumentNullException(nameof(actors));
        }

        public Guid Id { get; }
        public IEnumerable<Player> Players { get; }
        public IEnumerable<Actor> Actors { get => _actors; }

        public Actor GetActor(Guid actorId)
            => Actors.FirstOrDefault(a => a.Id == actorId);

        public void AddActor(Actor actor)
            => _actors = _actors.Add(actor);

        public void RemoveActor(Actor actor)
            => _actors = _actors.Remove(actor);

    }
}