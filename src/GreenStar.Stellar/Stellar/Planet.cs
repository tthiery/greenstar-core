using System;
using GreenStar.Core;
using GreenStar.Core.Traits;

namespace GreenStar.Stellar
{
    public class Planet : Actor
    {
        public Planet(Guid id)
            : base(id)
        {
            // A planet ..
            AddTrait<Locatable>(); // .. has a location 
            AddTrait<Hospitality>(); // .. can host actors
            AddTrait<Discoverable>(); // .. can be discovered by someone
            AddTrait<Populatable>(); //  .. can host a population
        }
    }
}