using System;
using System.Collections.Generic;
using GreenStar.Core;
using GreenStar.Core.Traits;

namespace GreenStar.Ships
{
    public abstract class Ship : Actor
    {
        public Ship(Guid id)
            : base(id)
        {
            // A ship ..
            AddTrait(new AssociatableTrait()); // .. has an owner
            AddTrait(new CapableTrait()); // .. has capabilities
            AddTrait(new DestructableTrait()); // .. can be destroyed
            AddTrait(new LocatableTrait()); // .. is located somewhere
            AddTrait(new DiscoverableTrait()); // .. can be discovered by someone
        }
    }
}