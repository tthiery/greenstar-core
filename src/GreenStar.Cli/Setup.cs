

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
using GreenStar.Transcripts;
using GreenStar.Ships.Factory;
using GreenStar.Research;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using GreenStar.Cartography.Builder;

namespace GreenStar.Cli;

public record GameType(string Name);

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
        => StellarSetup.FindAllStellarTypes();


    public (Guid, Guid) CreateGame(string selectedGameType, int nrOfSystemPlayers, StellarType selectedStellarType)
    {
        var rootDir = Path.Combine("../../data", selectedGameType);
        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, rootDir, "metrics.json"))
            .Build();

        var sp = new ServiceCollection()
            .Configure<PlanetLifeOptions>(config.GetSection("PlanetLife"))
            .Configure<ResearchOptions>(config.GetSection("Research"))
            // configure data loader
            .AddSingleton<ITechnologyDefinitionLoader>(new FileSystemTechnologyDefinitionLoader(rootDir))
            .AddSingleton<IPlayerTechnologyStateLoader>(new InMemoryPlayerTechnologyStateLoader())
            .AddSingleton<NameGenerator>(new NameGenerator()
                .Load("planet", Path.Combine(rootDir, "names-planet.json")))

            // intialize core services
            .AddSingleton<ResearchProgressEngine>()
            .AddSingleton<ShipFactory>()
            .BuildServiceProvider();

        var humanPlayer = new HumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1);

        var builder = new TurnManagerBuilder(sp)
            .AddCoreTranscript()
            .AddStellarTranscript()
            .AddElementsTranscript()
            .AddTranscript<ResearchSetup>(TurnTranscriptGroups.Setup)
            .AddTranscript(TurnTranscriptGroups.Setup, ActivatorUtilities.CreateInstance<StellarSetup>(sp, selectedStellarType))
            .AddTranscript<OccupationSetup>(TurnTranscriptGroups.Setup)
            .AddTranscript<InitialShipSetup>(TurnTranscriptGroups.Setup)
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

        for (int i = 0; i < stellarType.Arguments.Length; i++)
        {
            stellarType.Arguments[i] = stellarType.Arguments[i] with
            {
                Value = AnsiConsole.Ask<double>($"[green]{stellarType.Arguments[i].DisplayName}[/]", stellarType.Arguments[i].Value),
            };
        }

        return setupFacade.CreateGame(selectedGameType, nrOfSystemPlayers, stellarType);
    }

}
