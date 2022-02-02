using GreenStar;
using GreenStar.TurnEngine;

namespace GreenStar.TurnEngine;

public abstract class EventTranscript
{
    public abstract void Execute(Context context, Player player, string text, string[] arguments);
}
