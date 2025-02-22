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
using Microsoft.Extensions.FileProviders;

namespace GreenStar.AppService.Setup;

public class SetupDomainService : ISetupService
{
    public IEnumerable<GameType> GetGameTypes()
    {
        var fileProvider = new ManifestEmbeddedFileProvider(typeof(SetupDomainService).Assembly);

        return fileProvider.GetDirectoryContents("/").Where(d => d.IsDirectory).Select(d => new GameType(d.Name));
    }

    public IEnumerable<StellarType> GetStellarTypes()
        => StellarSetup.FindAllStellarTypes();

    public Task<IEnumerable<PersistedGame>> GetPersistedGamesAsync()
        => Task.FromResult<IEnumerable<PersistedGame>>(Directory
            .GetFiles(".", "save_core_*")
            .Select(s => s.Substring(0, s.IndexOf(".json")))
            .Select(s => s.Split("_"))
            .Select(s => new PersistedGame(Guid.Parse(s[3]), s[2], Guid.Empty)));


    public async Task<PersistedGame> CreateGameAsync(string selectedGameType, int nrOfAIPlayers, StellarType selectedStellarType)
    {
        var game = new PersistedGame(Guid.NewGuid(), selectedGameType, Guid.NewGuid());
        var (sp, builder) = BuildTurnEngine(game);

        // one time setup
        builder = builder
            .AddTranscript(TurnTranscriptGroups.Setup, ActivatorUtilities.CreateInstance<StellarSetup>(sp, selectedStellarType))
            .AddTranscript<OccupationSetup>(TurnTranscriptGroups.Setup)
            .AddTranscript<InitialShipSetup>(TurnTranscriptGroups.Setup);

        // one time player setup;
        var humanPlayer = new HumanPlayer();
        humanPlayer.Id = game.HumanPlayerId;
        humanPlayer.Relatable.PlayerId = humanPlayer.Id;
        humanPlayer.Relatable.ColorCode = "Red";
        humanPlayer.IdealConditions.IdealTemperature = 22;
        humanPlayer.IdealConditions.IdealGravity = 1.0;
        builder.AddPlayer(humanPlayer);

        for (int idx = 0; idx < nrOfAIPlayers; idx++)
        {
            var aiPlayer = new AIPlayer();
            aiPlayer.Id = Guid.NewGuid();
            aiPlayer.Relatable.PlayerId = aiPlayer.Id;
            aiPlayer.Relatable.ColorCode = "Blue";
            aiPlayer.IdealConditions.IdealTemperature = 22;
            aiPlayer.IdealConditions.IdealGravity = 1.0;

            builder.AddPlayer(aiPlayer);
        }

        // finalize turn engine build
        var turnEngine = await builder.BuildAsync();

        GameHolder.Games.Add(game.Id, turnEngine);

        return game;
    }

    public async Task<PersistedGame> LoadGameAsync(PersistedGame game)
    {
        var (sp, builder) = BuildTurnEngine(game);
        builder.AddTranscript(TurnTranscriptGroups.Setup, ActivatorUtilities.CreateInstance<LoadFromPersistenceSetup>(sp));

        // finalize turn engine build
        var turnEngine = await builder.BuildAsync();

        GameHolder.Games.Add(game.Id, turnEngine);

        var player = turnEngine.Players.GetAllPlayers().FirstOrDefault(p => p is HumanPlayer) ?? throw new NotImplementedException("no human player");

        return game with { HumanPlayerId = player.Id };
    }

    private static (ServiceProvider, TurnManagerBuilder) BuildTurnEngine(PersistedGame game)
    {
        var fileProvider = new ManifestEmbeddedFileProvider(typeof(SetupDomainService).Assembly);

        var gameConfigDir = game.Type;

        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(fileProvider, Path.Combine(game.Type, "metrics.json"), false, false)
            .Build();

        var sp = new ServiceCollection()
            .Configure<PlanetLifeOptions>(config.GetSection("PlanetLife"))
            .Configure<ResearchOptions>(config.GetSection("Research"))
            // configure data loader
            .AddSingleton<ActorTypeDictionary>(new ActorTypeDictionary(
                typeof(GreenStar.Stellar.Planet),
                typeof(GreenStar.Ships.Ship),
                typeof(GreenStar.Traits.IdealConditions)
            ))
            .AddSingleton<IRandomEventsLoader>(new FileSystemRandomEventsLoader(fileProvider, gameConfigDir))
            .AddSingleton<ITechnologyDefinitionLoader>(new FileSystemTechnologyDefinitionLoader(fileProvider, gameConfigDir))
            .AddSingleton<IPlayerTechnologyStateLoader>(new FileSystemPlayerTechnologyStateLoader(game.Id, game.Type))
            .AddSingleton<NameGenerator>(new NameGenerator()
                .Load("planet", fileProvider, Path.Combine(gameConfigDir, "names-planet.json")))
            .AddSingleton<IPersistence>(sp => new FileSystemPersistence(game.Id, game.Type, sp.GetService<ActorTypeDictionary>()))

            // intialize core services
            .AddSingleton<TechnologyProgressEngine>()
            .AddSingleton<ResearchProgressEngine>()
            .AddSingleton<ShipFactory>()
            .BuildServiceProvider();

        // game structure setup
        var builder = new TurnManagerBuilder(sp)
            .AddCoreTranscript()
            .AddEventTranscripts()
            .AddResearchTranscripts()
            .AddStellarTranscript()
            .AddElementsTranscript()
            .AddTranscript<PersistTurnTranscript>(TurnTranscriptGroups.EndTurn);

        return (sp, builder);
    }
}
