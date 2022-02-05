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
    void ExecuteCommand<TCommand>(Context context, Player player, TCommand command)
        where TCommand : Command;
}
