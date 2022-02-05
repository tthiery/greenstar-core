using GreenStar.TurnEngine;

namespace GreenStar;

public interface ITurnView
{
    int Turn { get; }
}
public interface ITurnContext
{
    int Turn { get; }
    void ExecuteEvent(Context context, Player player, string type, string argument, string text);
    void ExecuteCommand(Context context, Player player, Command command);
}
