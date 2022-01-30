

// TODO: against a facade (to allow later switch to a service layer)
// TODO: persistence
// TODO: game setup
// TODO: do turn
// TODO: see events
// TODO: see research tree
// TODO: execute commands
// ...

using GreenStar.Core;

using Spectre.Console;
using GreenStar.Core.TurnEngine;
using GreenStar.Algorithms;
using GreenStar.Core.TurnEngine.Players;
using System.Reflection;

namespace GreenStar.Cli;

public record GameType(string Name);
public record StellarType(string Name, StellarTypeProperty[] Properties);
public record StellarTypeProperty(string Name);

public static class GameHolder
{
    public static Dictionary<Guid, TurnManager> Games { get; } = new();
}

public class SetupFacade
{
    public IEnumerable<GameType> GetGameTypes()
    {
        return Directory.GetDirectories("../../data").Select(d => new GameType(new DirectoryInfo(d).Name));
    }

    public IEnumerable<StellarType> GetStellarTypes()
    {
        var t = Type.GetType("GreenStar.Algorithms.GeneratorAlgorithms, GreenStar.Stellar") ?? throw new InvalidOperationException("failed to find stellarstrategies");

        var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);

        var result = methods.Select(m => new StellarType(m.Name, m.GetParameters()
            .Where(p => p.Name != "actorContext")
            .Where(p => p.Name != "mode")
            .Select(p => new StellarTypeProperty(p.Name)).ToArray()));
        return result;
    }

    public (Guid, Guid) CreateGame(int nrOfSystemPlayers, string selectedStellarType, int[] stellarArgs)
    {
        var player = new HumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1);

        var builder = new TurnManagerBuilder()
            .AddCoreTranscript()
            .AddStellarTranscript()
            .AddPlayer(player);

        for (int idx = 0; idx < nrOfSystemPlayers; idx++)
        {
            builder.AddPlayer(new SystemPlayer());
        }
        var turnEngine = builder.Build();

        var t = Type.GetType("GreenStar.Algorithms.GeneratorAlgorithms, GreenStar.Stellar") ?? throw new InvalidOperationException("failed to find stellarstrategies");

        var method = t.GetMethod(selectedStellarType);
        var args = new object[2 + stellarArgs.Length];
        args[0] = (IActorContext)turnEngine.Game;
        args[1] = GeneratorMode.PlanetOnly;
        Array.Copy(stellarArgs, 0, args, 2, stellarArgs.Length);
        method.Invoke(null, args);

        var reference = Guid.NewGuid();

        GameHolder.Games.Add(reference, turnEngine);

        return (reference, player.Id);
    }
}

public static class Setup
{
    public static (Guid, Guid) SetupCommand()
    {
        var setupFacade = new SetupFacade();
        var gameTypes = setupFacade.GetGameTypes();
        var stellarTypes = setupFacade.GetStellarTypes();

        var selectedType = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Which Game [green]Type[/] you want to start?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
            .AddChoices(gameTypes.Select(g => g.Name)));

        AnsiConsole.WriteLine($"Game Type: {selectedType}");

        var nrOfSystemPlayers = AnsiConsole.Ask<int>("How many System Players?");

        var selectedStellarType = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Define the [green]stellar type[/] you want to play?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
            .AddChoices(stellarTypes.Select(g => g.Name)));
        AnsiConsole.WriteLine($"Stellar Type: {selectedStellarType}");

        var stellarType = stellarTypes.First(t => t.Name == selectedStellarType);

        var stellarArgs = new int[stellarType.Properties.Length];

        for (int i = 0; i < stellarType.Properties.Length; i++)
        {
            stellarArgs[i] = AnsiConsole.Ask<int>($"[green]{stellarType.Properties[i].Name}[/]");

        }

        return setupFacade.CreateGame(nrOfSystemPlayers, selectedStellarType, stellarArgs);
    }

}
