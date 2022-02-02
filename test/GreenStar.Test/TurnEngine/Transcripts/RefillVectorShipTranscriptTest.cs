using System;

using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine.Players;

using Xunit;

namespace GreenStar.TurnEngine.Transcripts;

public class RefillVectorShipTranscriptTest
{
    [Fact]
    public void RefillVectorShipTranscript_Turn_NoRefillOnExactLocation()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new ExactLocation();
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Scout>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(1, ship.Trait<VectorFlightCapable>().Fuel);
    }

    [Fact]
    public void RefillVectorShipTranscript_Turn_RefillOnOwnPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Scout>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(10, ship.Trait<VectorFlightCapable>().Fuel);
    }

    [Fact]
    public void RefillVectorShipTranscript_Turn_RefillOnFriendlyPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p2;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Scout>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(10, ship.Trait<VectorFlightCapable>().Fuel);
    }

    [Fact]
    public void RefillVectorShipTranscript_Turn_NoRefillOnOtherPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p3;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Scout>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(1, ship.Trait<VectorFlightCapable>().Fuel);
    }

    [Fact]
    public void RefillVectorShipTranscript_Turn_RefillOnOwnPlanetForTankerSuperfil()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Tanker>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(100, ship.Trait<VectorFlightCapable>().Fuel);
    }

    [Fact]
    public void RefillVectorShipTranscript_Turn_RefillOnExactLocationFromTanker()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new ExactLocation();
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Scout>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        var tanker = context.ActorContext.AddShip<Tanker>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(tanker);
        tanker.Trait<VectorFlightCapable>().Fuel = 100;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(10, ship.Trait<VectorFlightCapable>().Fuel);
        // assert
        Assert.Equal(91, tanker.Trait<VectorFlightCapable>().Fuel);
    }


    [Fact]
    public void RefillVectorShipTranscript_Turn_NoRefillOnExactLocationFromTankerOfOtherPlayer()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new ExactLocation();
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Scout>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        var tanker = context.ActorContext.AddShip<Tanker>(p2, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(tanker);
        tanker.Trait<VectorFlightCapable>().Fuel = 100;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(1, ship.Trait<VectorFlightCapable>().Fuel);
        // assert
        Assert.Equal(100, tanker.Trait<VectorFlightCapable>().Fuel);
    }


    [Fact]
    public void RefillVectorShipTranscript_Turn_NoRefillForBioshipOnExactLocation()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new ExactLocation();
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Bioship>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(1, ship.Trait<VectorFlightCapable>().Fuel);
    }


    [Fact]
    public void RefillVectorShipTranscript_Turn_RefillForBioshipOnOwnPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        location.Trait<Populatable>().Population = 10000;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Bioship>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(10, ship.Trait<VectorFlightCapable>().Fuel);
        Assert.Equal(1000, location.Trait<Populatable>().Population);
    }

    [Fact]
    public void RefillVectorShipTranscript_Turn_RefillForBioshipOnOtherPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = CreateEnvironment();
        var context = turnManager.CreateTurnContext();

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p3;
        location.Trait<Populatable>().Population = 10000;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<Bioship>(p1, range: 10, speed: 5);
        location.Trait<Hospitality>().Enter(ship);
        ship.Trait<VectorFlightCapable>().Fuel = 1;

        // act
        turnManager.FinishTurnForAllPlayers();

        // assert
        Assert.Equal(10, ship.Trait<VectorFlightCapable>().Fuel);
        Assert.Equal(1000, location.Trait<Populatable>().Population);
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
