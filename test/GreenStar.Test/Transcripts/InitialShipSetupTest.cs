using System;
using System.Linq;

using GreenStar.Research;
using GreenStar.Ships;
using GreenStar.Ships.Factory;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Xunit;

namespace GreenStar.Transcripts;

public class InitialShipSetupTest
{
    [Fact]
    public void InitialShipSetup_Execute_Simple()
    {
        // arrange
        var technologyDefinitionLoader = new FileSystemTechnologyDefinitionLoader("../../../../../data/testgame");
        var playerTechnologyStateLoader = new InMemoryPlayerTechnologyStateLoader();
        var shipFactory = new ShipFactory(playerTechnologyStateLoader);
        var researchManager = new ResearchProgressEngine(technologyDefinitionLoader);

        var humanPlayer = new HumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1);

        var builder = new TurnManagerBuilder()
            .AddCoreTranscript()
            .AddStellarTranscript()
            .AddTranscript(TurnTranscriptGroups.Setup, new ResearchSetup(researchManager, playerTechnologyStateLoader))
            .AddTranscript(TurnTranscriptGroups.Setup, new StellarSetup("SolarSystem", new[] { 1 }))
            .AddTranscript(TurnTranscriptGroups.Setup, new OccupationSetup())
            .AddPlayer(humanPlayer);

        // act
        var turnEngine = builder
            .AddTranscript(TurnTranscriptGroups.Setup, new InitialShipSetup(shipFactory))
            .Build();

        // assert
        var list = turnEngine.Actors.AsQueryable().OfType<Ship>();
        Assert.Collection(list,
            a => { Assert.IsType<Scout>(a); Assert.Equal(humanPlayer.Id, a.Trait<Associatable>().PlayerId); },
            a => { Assert.IsType<Scout>(a); Assert.Equal(humanPlayer.Id, a.Trait<Associatable>().PlayerId); },
            a => { Assert.IsType<ColonizeShip>(a); Assert.Equal(humanPlayer.Id, a.Trait<Associatable>().PlayerId); }
        );

    }
}