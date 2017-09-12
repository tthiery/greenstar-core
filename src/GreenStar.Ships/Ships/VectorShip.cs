using System;
using System.Collections.Generic;
using GreenStar.Core;
using GreenStar.Core.Traits;

namespace GreenStar.Ships
{
    public class VectorShip : Ship
    {
        public VectorShip(Guid id)
            : base(id)
        {
            // A vector ship ..
            AddTrait(new VectorFlightCapableTrait()); // .. can initialize a vector flight
            AddTrait(new CommandableTrait()); // .. has commands
        }
    }
}