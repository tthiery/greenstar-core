using System;
using GreenStar.Core;
using GreenStar.Core.Traits;

namespace GreenStar.Stellar
{
    public class Sun : Actor
    {
        public Sun()
        {
            // A sun ..
            AddTrait<Locatable>(); // .. has a location
            AddTrait<Hospitality>(); // .. can host actors
            AddTrait<Discoverable>(); // .. can be discovered by someone
        }
    }
}