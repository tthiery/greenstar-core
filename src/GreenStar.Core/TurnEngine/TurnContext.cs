using System;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace GreenStar.TurnEngine;

public class TurnContext : ITurnContext, ITurnView
{
    private readonly IServiceProvider _serviceProvider;

    public TurnContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public int Turn { get; set; } = 0;

    public async Task ExecuteCommandAsync(Context context, Player player, Command command)
    {
        var genericBaseType = typeof(CommandTranscript<>);
        var baseType = genericBaseType.MakeGenericType(command.GetType());
        var transcriptType = AssemblyLoadContext.Default.Assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsAssignableTo(baseType))
            .FirstOrDefault()
            ?? throw new ArgumentException("cannot find executor for provided command", nameof(command));

        var transcript = ActivatorUtilities.CreateInstance(_serviceProvider, transcriptType, new object[] { command }) as Transcript
            ?? throw new InvalidOperationException("cannot instanciate executor for provided command");

        await transcript.ExecuteAsync(context with
        {
            Player = player,
        });
    }

    public async Task ExecuteEventAsync(Context context, Player player, string type, string argument, string text)
    {
        var t = Type.GetType(type);

        if (t is not null)
        {
            string[] args = argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var constructorArguments = new object[2];
            constructorArguments[0] = text;
            constructorArguments[1] = args;

            var eventExecutor = ActivatorUtilities.CreateInstance(_serviceProvider, t, constructorArguments) as EventTranscript
                ?? throw new InvalidOperationException("lost type between calls?");

            await eventExecutor.ExecuteAsync(context with
            {
                Player = player,
            });
        }
    }
}
