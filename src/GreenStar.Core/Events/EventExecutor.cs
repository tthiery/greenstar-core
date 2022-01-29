using System;

using GreenStar.Core;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Events;

public static class EventExecutor
{
    public static void Execute(Context context, Player player, string type, string argument, string text)
    {
        var t = Type.GetType(type);

        if (t is not null)
        {
            var eventExecutor = Activator.CreateInstance(t) as IEventExecutor ?? throw new InvalidOperationException("lost type between calls?");

            string[] args = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            eventExecutor.Execute(context, player, text, args);
        }
    }
}