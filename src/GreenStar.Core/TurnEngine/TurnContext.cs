using System;
using System.Linq;
using System.Runtime.Loader;

namespace GreenStar.TurnEngine;

public class TurnContext : ITurnContext, ITurnView
{
    public int Turn { get; set; } = 0;

    public void ExecuteCommand<TCommand>(Context context, Player player, TCommand command)
        where TCommand : Command
    {
        var baseType = typeof(CommandTranscript<TCommand>);
        var transcriptType = AssemblyLoadContext.Default.Assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(baseType))
            .FirstOrDefault()
            ?? throw new ArgumentException("cannot find executor for provided command", nameof(command));

        var transcript = Activator.CreateInstance(transcriptType, new object[] { command }) as CommandTranscript<TCommand>
            ?? throw new InvalidOperationException("cannot instanciate executor for provided command");

        transcript.Execute(context with
        {
            Player = player,
        });
    }

    public void ExecuteEvent(Context context, Player player, string type, string argument, string text)
    {
        var t = Type.GetType(type);

        if (t is not null)
        {
            string[] args = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var constructorArguments = new object[2];
            constructorArguments[0] = text;
            constructorArguments[1] = args;

            var eventExecutor = Activator.CreateInstance(t, constructorArguments) as EventTranscript
                ?? throw new InvalidOperationException("lost type between calls?");

            eventExecutor.Execute(context with
            {
                Player = player,
            });
        }
    }
}
