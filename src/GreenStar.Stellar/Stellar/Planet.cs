using System;

using GreenStar;
using GreenStar.Traits;

namespace GreenStar.Stellar;

public class Planet : Actor
{
    public Planet()
    {
        // A planet ..
        AddTrait<Locatable>(); // .. has a location
        AddTrait<Orbiting>(); // .. with a orbit around a sun
        AddTrait<Associatable>(); // .. can be associated to a player
        AddTrait<Hospitality>(); // .. can host actors
        AddTrait<Discoverable>(); // .. can be discovered by someone
        AddTrait<Populatable>(); //  .. can host a population
        AddTrait<Resourceful>(); // .. has resources
    }
}
