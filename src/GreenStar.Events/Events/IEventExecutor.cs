using GreenStar.Core;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Events
{
    public interface IEventExecutor
    {
        void Execute(Context context, Player player, string argument, string text);
    }
}