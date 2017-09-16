using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public void AddTrait<T>(params object[] config) where T: Trait
        {
            int i = 0;
            var type = typeof(T);
            var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();

            var constructorParameter = constructor.GetParameters()
                .Select(p => p.ParameterType)
                .Select(t => Traits.FirstOrDefault(trait => trait.GetType() == t))
                .Select(v => v ?? config[i++])
                .ToArray();

            if (constructorParameter.Any(cp => cp == null)) {
                throw new InvalidOperationException("Cannot find a parameter for a trait.");
            }

            var newTrait = Activator.CreateInstance(type, constructorParameter) as T;

            AddTrait(newTrait);
        }

        public T Trait<T>() where T : Trait
            => _traits.FirstOrDefault(t => t is T) as T;
        public bool HasTrait<T>() where T : Trait
            => _traits.Any(t => t is T);
    }
}