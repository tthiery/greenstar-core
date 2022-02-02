using GreenStar.Traits;

namespace GreenStar.Ships;

public class ColonizeShip : VectorShip
{
    public ColonizeShip()
    {
        AddTrait<ColonizationCapable>();
    }
}
