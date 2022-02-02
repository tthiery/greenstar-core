using GreenStar;

namespace GreenStar.Ships;

public class Satellite : Ship
{
    public Satellite()
        : base(new string[] {
                ShipCapabilities.Attack,
                ShipCapabilities.Defense,
                ShipCapabilities.Mini,
        })
    {

    }
}
