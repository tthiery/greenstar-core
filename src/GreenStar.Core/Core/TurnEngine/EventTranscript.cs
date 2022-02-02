using GreenStar.Core;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core.TurnEngine;

public abstract class EventTranscript
{
    public abstract void Execute(Context context, Player player, string text, string[] arguments);
}
