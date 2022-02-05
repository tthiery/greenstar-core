using System;

namespace GreenStar.TurnEngine;

public class TurnContext : ITurnContext, ITurnView
{
    public int Turn { get; set; } = 0;

    public void Execute(Context context, Player player, string type, string argument, string text)
    {
        var t = Type.GetType(type);

        if (t is not null)
        {
            string[] args = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var constructorArguments = new object[2];
            constructorArguments[0] = text;
            constructorArguments[1] = args;

            var eventExecutor = Activator.CreateInstance(t, constructorArguments) as EventTranscript ?? throw new InvalidOperationException("lost type between calls?");

            eventExecutor.Execute(context with
            {
                Player = player,
            });
        }
    }
}
