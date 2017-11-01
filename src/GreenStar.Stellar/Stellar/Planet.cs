using System;
using GreenStar.Core;
using GreenStar.Core.Traits;

namespace GreenStar.Stellar
{
    public class Planet : Actor
    {
        public Planet()
        {
            // A planet ..
            AddTrait<Locatable>(); // .. has a location ^
            AddTrait<Associatable>(); // .. can be associated to a player
            AddTrait<Hospitality>(); // .. can host actors
            AddTrait<Discoverable>(); // .. can be discovered by someone
            AddTrait<Populatable>(); //  .. can host a population
            AddTrait<Resourceful>(); // .. has resources
        }
    }
}