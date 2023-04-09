using System;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.Cartography.Builder;
using GreenStar.Research;
using GreenStar.Ships;
using GreenStar.Ships.Factory;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using static GreenStar.Test.Helper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit;

namespace GreenStar.Transcripts;

public class InitialShipSetupTest
{
    [Fact]
    public async Task InitialShipSetup_Execute_Simple()
    {
        // arrange
        var nameGenerator = new NameGenerator();
        var technologyDefinitionLoader = new FileSystemTechnologyDefinitionLoader("../../../../../data/testgame");
        var playerTechnologyStateLoader = new InMemoryPlayerTechnologyStateLoader();
        var shipFactory = new ShipFactory(playerTechnologyStateLoader);
        var researchManager = new TechnologyProgressEngine(technologyDefinitionLoader);

        var humanPlayer = CreateHumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1);

        var sp = new ServiceCollection()
            .Configure<PlanetLifeOptions>(_ => { })
            .AddSingleton(nameGenerator)
            .BuildServiceProvider();

        var builder = new TurnManagerBuilder(sp)
            .AddCoreTranscript()
            .AddStellarTranscript()
            .AddTranscript(TurnTranscriptGroups.Setup, new TechnologySetup(researchManager, playerTechnologyStateLoader))
            .AddTranscript(TurnTranscriptGroups.Setup, new StellarSetup(nameGenerator, sp, new StellarType("SolarSystem", string.Empty, new[] { new StellarGeneratorArgument("planetCount", "", 1) })))
            .AddTranscript(TurnTranscriptGroups.Setup, new OccupationSetup(Options.Create(new PlanetLifeOptions())))
            .AddPlayer(humanPlayer);

        // act
        var turnEngine = await builder
            .AddTranscript(TurnTranscriptGroups.Setup, new InitialShipSetup(shipFactory))
            .BuildAsync();

        // assert
        var list = turnEngine.Actors.AsQueryable().OfType<Ship>();
        Assert.Collection(list,
            a => { Assert.IsType<Scout>(a); Assert.Equal(humanPlayer.Id, a.Trait<Associatable>().PlayerId); },
            a => { Assert.IsType<Scout>(a); Assert.Equal(humanPlayer.Id, a.Trait<Associatable>().PlayerId); },
            a => { Assert.IsType<ColonizeShip>(a); Assert.Equal(humanPlayer.Id, a.Trait<Associatable>().PlayerId); }
        );

    }
}