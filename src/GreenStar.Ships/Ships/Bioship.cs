using GreenStar.Core.Traits;

namespace GreenStar.Ships;

public class Bioship : VectorShip
{
    public Bioship()
    {
        Trait<VectorFlightCapable>().FuelType = Fuels.Biomass;
    }
}
