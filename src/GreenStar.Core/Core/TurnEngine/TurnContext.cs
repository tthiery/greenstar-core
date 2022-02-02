using System;

namespace GreenStar.Core.TurnEngine;

public class TurnContext : ITurnContext, ITurnView
{
    public int Turn { get; set; } = 0;

    public void Execute(Context context, Player player, string type, string argument, string text)
    {
        var t = Type.GetType(type);

        if (t is not null)
        {
            var eventExecutor = Activator.CreateInstance(t) as EventTranscript ?? throw new InvalidOperationException("lost type between calls?");

            string[] args = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            eventExecutor.Execute(context, player, text, args);
        }
    }
}
