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
            AddTrait(new LocatableTrait()); // .. has a location 
            AddTrait(new HostTrait()); // .. can host actors
            AddTrait(new DiscoverableTrait()); // .. can be discovered by someone
        }
    }
}