using GreenStar.Core.TurnEngine;

namespace GreenStar.Core;

public interface ITurnView
{
    int Turn { get; }
}
public interface ITurnContext
{
    int Turn { get; }
    void Execute(Context context, Player player, string type, string argument, string text);
}
