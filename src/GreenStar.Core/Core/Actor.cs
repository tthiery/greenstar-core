using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar.Core
{
    public class Actor
    {
        private List<Trait> _traits;
        public Actor(Guid id)
        {
            Id = id;
            _traits = new List<Trait>();
        }

        public Guid Id { get; }

        public IEnumerable<Trait> Traits { get => _traits; }

        public void AddTrait(Trait trait)
        {
            trait.Self = this;
            _traits.Add(trait);
        }

        public T Trait<T>() where T : Trait
            => _traits.FirstOrDefault(t => t is T) as T;
        public bool HasTrait<T>() where T : Trait
            => _traits.Any(t => t is T);
    }
}