using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using GreenStar.Core.Persistence;

namespace GreenStar.Core;

public class Actor
{
    private readonly List<Trait> _traits;

    public Guid Id { get; set; }

    public IEnumerable<Trait> Traits
        => _traits;

    public Actor()
    {
        Id = Guid.NewGuid();
        _traits = new List<Trait>();
    }

    public void AddTrait(Trait trait)
    {
        trait.Self = this;
        _traits.Add(trait);
    }

    public T AddTrait<T>(params object[] config) where T : Trait
    {
        int i = 0;
        var type = typeof(T);
        var constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();

        var constructorParameter = constructor?.GetParameters()
            .Select(p => p.ParameterType)
            .Select(t => Traits.FirstOrDefault(trait => trait.GetType() == t))
            .Select(v => v ?? config[i++])
            .ToArray();

        if (constructorParameter?.Any(cp => cp == null) ?? true)
        {
            throw new InvalidOperationException("Cannot find a parameter for a trait.");
        }

        var newTrait = Activator.CreateInstance(type, constructorParameter) as T ?? throw new InvalidOperationException("Unable to create instance");

        AddTrait(newTrait);

        return newTrait;
    }

    public T Trait<T>() where T : Trait
        => TryGetTrait<T>(out var result) ? result : throw new InvalidOperationException("query invalid trait");

    public bool TryGetTrait<T>([NotNullWhen(returnValue: true)] out T? trait)
        where T : Trait
    {
        trait = _traits.FirstOrDefault(t => t is T) as T;

        return trait is not null;
    }
    public bool HasTrait<T>() where T : Trait
        => _traits.Any(t => t is T);
}
