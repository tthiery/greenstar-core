using System;

using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine.Players;
using GreenStar.Ships;
using GreenStar.Stellar;

using Xunit;

namespace GreenStar.Core.TurnEngine.Transcripts;

public class ColonizeTranscriptTest
{
    [Fact]
    public void ColonizeTranscript_Turn_ColonizeEmptyPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = Guid.Empty;
        location.Trait<Populatable>().Population = 0;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        location.Trait<Hospitality>().Enter(ship);

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(p1, location.Trait<Associatable>().PlayerId);
        Assert.Equal(10, location.Trait<Populatable>().Population);
    }

    [Fact]
    public void ColonizeTranscript_Turn_NoColonizeOnPopulatedOwnPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        location.Trait<Populatable>().Population = 2000;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        location.Trait<Hospitality>().Enter(ship);

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(p1, location.Trait<Associatable>().PlayerId);
        Assert.Equal(2000, location.Trait<Populatable>().Population);
    }

    [Fact]
    public void ColonizeTranscript_Turn_NoColonizeOnPopulatedOtherPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p3;
        location.Trait<Populatable>().Population = 2000;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        location.Trait<Hospitality>().Enter(ship);

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(p3, location.Trait<Associatable>().PlayerId);
        Assert.Equal(2000, location.Trait<Populatable>().Population);
    }

    [Fact]
    public void ColonizeTranscript_Turn_NoColonizeOnWithoutColonists()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = Guid.Empty;
        location.Trait<Populatable>().Population = 0;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        ship.Trait<ColonizationCapable>().IsLoaded = false;
        location.Trait<Hospitality>().Enter(ship);

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(Guid.Empty, location.Trait<Associatable>().PlayerId);
        Assert.Equal(0, location.Trait<Populatable>().Population);
        Assert.False(ship.Trait<ColonizationCapable>().IsLoaded);
    }

    [Fact]
    public void ColonizeTranscript_Turn_RecruitColonists()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        location.Trait<Populatable>().Population = 1234;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        ship.Trait<ColonizationCapable>().IsLoaded = false;
        location.Trait<Hospitality>().Enter(ship);

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(p1, location.Trait<Associatable>().PlayerId);
        Assert.Equal(1234, location.Trait<Populatable>().Population);
        Assert.True(ship.Trait<ColonizationCapable>().IsLoaded);
    }

    public (TurnManager turnManager, Guid p1, Guid p2, Guid p3) CreateEnvironment()
    {
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        var p3 = Guid.NewGuid();

        var turnManager = new TurnManagerBuilder()
            .AddPlayer(new HumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1))
            .AddPlayer(new HumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1))
            .AddPlayer(new HumanPlayer(p3, "orange", new Guid[] { }, 20, 1))
            .AddElementsTranscript()
            .Build();

        return (turnManager, p1, p2, p3);
    }
}
