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
            AddTrait<Associatable>(); // .. has an owner
            AddTrait<Capable>(); // .. has capabilities
            AddTrait<Destructable>(); // .. can be destroyed
            AddTrait<Locatable>(); // .. is located somewhere
            AddTrait<Discoverable>(); // .. can be discovered by someone
        }
    }
}