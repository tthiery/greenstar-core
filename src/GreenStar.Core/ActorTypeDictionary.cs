using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar;

public class ActorTypeDictionary
{
    public ActorTypeDictionary(params Type[] typesOfAssembliesToDiscover)
    {
        var actorBaseType = typeof(Actor);
        var traitBaseType = typeof(Trait);
        var actors = new List<Type>();
        var traits = new List<Type>();
        foreach (var t in typesOfAssembliesToDiscover)
        {
            var allTypes = t.Assembly.GetTypes();

            actors.AddRange(allTypes.Where(tia => tia.IsSubclassOf(actorBaseType)));
            traits.AddRange(allTypes.Where(tia => tia.IsSubclassOf(traitBaseType)));
        }

        Actors = actors.ToDictionary(t => t.Name, t => t).ToFrozenDictionary();
        Traits = traits.ToDictionary(t => t.Name, t => t).ToFrozenDictionary();
    }

    public FrozenDictionary<string, Type> Actors { get; }
    public FrozenDictionary<string, Type> Traits { get; }
}