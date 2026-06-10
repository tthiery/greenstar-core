using GreenStar.Traits;

namespace GreenStar;

public class SelectedItem
    : Actor
{
    public SelectedItem()
    {
        AddTrait<Locatable>();
    }
}