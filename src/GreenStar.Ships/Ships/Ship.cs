using System;
using System.Collections.Generic;

using GreenStar;
using GreenStar.Traits;

namespace GreenStar.Ships;

public abstract class Ship : Actor
{
    public Ship(string[] technologyNames)
    {
        // A ship ..
        AddTrait<Associatable>(); // .. has an owner
        AddTrait<Capable>((object)technologyNames); // .. has capabilities
        AddTrait<Destructable>(); // .. can be destroyed
        AddTrait<Locatable>(); // .. is located somewhere
        AddTrait<Discoverable>(); // .. can be discovered by someone
    }
}
