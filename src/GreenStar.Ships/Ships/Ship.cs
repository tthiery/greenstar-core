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
            AddTrait(new Associatable()); // .. has an owner
            AddTrait(new Capable()); // .. has capabilities
            AddTrait(new Destructable()); // .. can be destroyed
            AddTrait(new Locatable()); // .. is located somewhere
            AddTrait(new Discoverable()); // .. can be discovered by someone
        }
    }
}