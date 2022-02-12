using System.Threading.Tasks;

using GreenStar.TurnEngine;

namespace GreenStar;

public interface ITurnView
{
    int Turn { get; }
}
public interface ITurnContext
{
    int Turn { get; }
    Task ExecuteEventAsync(Context context, Player player, string type, string[] argument, string text);
    Task ExecuteCommandAsync(Context context, Player player, Command command);
}
