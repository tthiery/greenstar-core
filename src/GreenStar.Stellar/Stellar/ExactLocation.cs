using System;
using GreenStar.Core;
using GreenStar.Core.Traits;

namespace GreenStar.Stellar
{
    public class ExactLocation : Actor
    {
        public ExactLocation(Guid id)
            : base(id)
        {
            // An exact location ..
            AddTrait(new Locatable()); // .. has a location 
            AddTrait(new Hospitality()); // .. can host actors
            AddTrait(new Discoverable()); // .. can be discovered by someone
        }
    }
}