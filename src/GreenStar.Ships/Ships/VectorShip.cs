using System;
using System.Collections.Generic;

using GreenStar;
using GreenStar.Traits;

namespace GreenStar.Ships;

public class VectorShip : Ship
{
    public VectorShip()
        : base(new string[] {
                ShipCapabilities.Range,
                ShipCapabilities.Speed,
                ShipCapabilities.Attack,
                ShipCapabilities.Defense,
                ShipCapabilities.Mini,
        })
    {
        // A vector ship ..
        AddTrait<Commandable>(); // .. has commands
        AddTrait<VectorFlightCapable>(); // .. can initialize a vector flight
    }
}
