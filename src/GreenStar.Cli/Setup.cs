

// TODO: against a facade (to allow later switch to a service layer)
// TODO: persistence
// TODO: game setup
// TODO: do turn
// TODO: see events
// TODO: see research tree
// TODO: execute commands
// ...

using GreenStar.Algorithms;
using GreenStar.Cartography.Builder;
using GreenStar.Events;
using GreenStar.Persistence;
using GreenStar.Research;
using GreenStar.Ships.Factory;
using GreenStar.Transcripts;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

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


    public async Task<(Guid, Guid)> CreateGameAsync(string selectedGameType, int nrOfAIPlayers, StellarType selectedStellarType)
    {
        var gameConfigDir = Path.Combine("../../data", selectedGameType);
        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, gameConfigDir, "metrics.json"))
            .Build();

        var sp = new ServiceCollection()
            .Configure<PlanetLifeOptions>(config.GetSection("PlanetLife"))
            .Configure<ResearchOptions>(config.GetSection("Research"))
            // configure data loader
            .AddSingleton<IRandomEventsLoader>(new FileSystemRandomEventsLoader(gameConfigDir))
            .AddSingleton<ITechnologyDefinitionLoader>(new FileSystemTechnologyDefinitionLoader(gameConfigDir))
            .AddSingleton<IPlayerTechnologyStateLoader>(new InMemoryPlayerTechnologyStateLoader())
            .AddSingleton<NameGenerator>(new NameGenerator()
                .Load("planet", Path.Combine(gameConfigDir, "names-planet.json")))
            .AddSingleton<IPersistence>(new FileSystemPersistence())

            // intialize core services
            .AddSingleton<TechnologyProgressEngine>()
            .AddSingleton<ResearchProgressEngine>()
            .AddSingleton<ShipFactory>()
            .BuildServiceProvider();

        var humanPlayer = new HumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1);

        var builder = new TurnManagerBuilder(sp)
            // game structure setup
            .AddCoreTranscript()
            .AddEventTranscripts()
            .AddResearchTranscripts()
            .AddStellarTranscript()
            .AddElementsTranscript()
            .AddTranscript<PersistActorsTurnTranscript>(TurnTranscriptGroups.EndTurn)

            // one time setup
            .AddTranscript(TurnTranscriptGroups.Setup, ActivatorUtilities.CreateInstance<StellarSetup>(sp, selectedStellarType))
            .AddTranscript<OccupationSetup>(TurnTranscriptGroups.Setup)
            .AddTranscript<InitialShipSetup>(TurnTranscriptGroups.Setup)
            .AddPlayer(humanPlayer);

        for (int idx = 0; idx < nrOfAIPlayers; idx++)
        {
            builder.AddPlayer(new AIPlayer(Guid.NewGuid(), "Blue", Array.Empty<Guid>(), 22, 1));
        }
        var turnEngine = await builder.BuildAsync();

        var reference = Guid.NewGuid();

        GameHolder.Games.Add(reference, turnEngine);

        return (reference, humanPlayer.Id);
    }
}

public static class Setup
{
    public static async Task<(Guid, Guid)> SetupCommand()
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

        return await setupFacade.CreateGameAsync(selectedGameType, nrOfSystemPlayers, stellarType);
    }

}
