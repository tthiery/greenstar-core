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

namespace GreenStar.AppService.Setup;

public class SetupDomainService : ISetupService
{
    public IEnumerable<GameType> GetGameTypes()
    {
        return Directory.GetDirectories("../../data").Select(d => new GameType(new DirectoryInfo(d).Name));
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
        var gameConfigDir = Path.Combine("../../data", game.Type);
        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile(Path.Combine(Environment.CurrentDirectory, gameConfigDir, "metrics.json"))
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
            .AddSingleton<IRandomEventsLoader>(new FileSystemRandomEventsLoader(gameConfigDir))
            .AddSingleton<ITechnologyDefinitionLoader>(new FileSystemTechnologyDefinitionLoader(gameConfigDir))
            .AddSingleton<IPlayerTechnologyStateLoader>(new FileSystemPlayerTechnologyStateLoader(game.Id, game.Type))
            .AddSingleton<NameGenerator>(new NameGenerator()
                .Load("planet", Path.Combine(gameConfigDir, "names-planet.json")))
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
