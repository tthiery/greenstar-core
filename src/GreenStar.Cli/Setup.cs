

// TODO: against a facade (to allow later switch to a service layer)
// TODO: persistence
// TODO: game setup
// TODO: do turn
// TODO: see events
// TODO: see research tree
// TODO: execute commands
// ...

using GreenStar;

using Spectre.Console;
using GreenStar.TurnEngine;
using GreenStar.Algorithms;
using GreenStar.TurnEngine.Players;
using System.Reflection;
using GreenStar.TurnEngine.Transcripts;
using GreenStar.Cli.Adapter;
using GreenStar.Ships.Factory;
using GreenStar.Research;

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

    public (Guid, Guid) CreateGame(string selectedGameType, int nrOfSystemPlayers, string selectedStellarType, int[] stellarArgs)
    {
        var technologyDefinitionLoader = new FileSystemTechnologyDefinitionLoader("../../data/" + selectedGameType);
        var playerTechnologyStateLoader = new InMemoryPlayerTechnologyStateLoader();
        var shipFactory = new ShipFactory(playerTechnologyStateLoader);
        var researchManager = new ResearchProgressEngine(technologyDefinitionLoader);

        var humanPlayer = new HumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1);

        var builder = new TurnManagerBuilder()
            .AddCoreTranscript()
            .AddStellarTranscript()
            .AddTranscript(TurnTranscriptGroups.Setup, new ResearchSetup(researchManager, playerTechnologyStateLoader))
            .AddTranscript(TurnTranscriptGroups.Setup, new StellarSetup(selectedStellarType, stellarArgs))
            .AddTranscript(TurnTranscriptGroups.Setup, new OccupationSetup())
            .AddTranscript(TurnTranscriptGroups.Setup, new InitialShipSetup(shipFactory))
            .AddPlayer(humanPlayer);

        for (int idx = 0; idx < nrOfSystemPlayers; idx++)
        {
            builder.AddPlayer(new AIPlayer(Guid.NewGuid(), "Blue", Array.Empty<Guid>(), 22, 1));
        }
        var turnEngine = builder.Build();

        var reference = Guid.NewGuid();

        GameHolder.Games.Add(reference, turnEngine);

        return (reference, humanPlayer.Id);
    }
}

public static class Setup
{
    public static (Guid, Guid) SetupCommand()
    {
        var setupFacade = new SetupFacade();
        var gameTypes = setupFacade.GetGameTypes();
        var stellarTypes = setupFacade.GetStellarTypes();

        var selectedGameType = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Which Game [green]Type[/] you want to start?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
            .AddChoices(gameTypes.Select(g => g.Name)));

        AnsiConsole.WriteLine($"Game Type: {selectedGameType}");

        var nrOfSystemPlayers = AnsiConsole.Ask<int>("How many AI Players?");

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

        return setupFacade.CreateGame(selectedGameType, nrOfSystemPlayers, selectedStellarType, stellarArgs);
    }

}
