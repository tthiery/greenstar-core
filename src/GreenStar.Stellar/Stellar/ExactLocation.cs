using System;

using GreenStar;
using GreenStar.Traits;

namespace GreenStar.Stellar;

public class ExactLocation : Actor
{
    public ExactLocation()
    {
        // An exact location ..
        AddTrait<Nameable>(); // .. has a name
        AddTrait<Locatable>(); // .. has a location 
        AddTrait<Hospitality>(); // .. can host actors
        AddTrait<Discoverable>(); // .. can be discovered by someone
    }
}
