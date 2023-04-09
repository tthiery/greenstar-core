using System;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using static GreenStar.Test.Helper;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Transcripts;

public class ColonizeTranscriptTest
{
    [Fact]
    public async Task ColonizeTranscript_Turn_ColonizeEmptyPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();
        var context = turnManager.CreateTurnContext(p1);

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = Guid.Empty;
        location.Trait<Populatable>().Population = 0;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        location.Trait<Hospitality>().Enter(ship);

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(p1, location.Trait<Associatable>().PlayerId);
        Assert.Equal(10, location.Trait<Populatable>().Population);
    }

    [Fact]
    public async Task ColonizeTranscript_Turn_NoColonizeOnPopulatedOwnPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();
        var context = turnManager.CreateTurnContext(p1);

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        location.Trait<Populatable>().Population = 2000;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        location.Trait<Hospitality>().Enter(ship);

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(p1, location.Trait<Associatable>().PlayerId);
        Assert.Equal(2000, location.Trait<Populatable>().Population);
    }

    [Fact]
    public async Task ColonizeTranscript_Turn_NoColonizeOnPopulatedOtherPlanet()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();
        var context = turnManager.CreateTurnContext(p1);

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p3;
        location.Trait<Populatable>().Population = 2000;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        location.Trait<Hospitality>().Enter(ship);

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(p3, location.Trait<Associatable>().PlayerId);
        Assert.Equal(2000, location.Trait<Populatable>().Population);
    }

    [Fact]
    public async Task ColonizeTranscript_Turn_NoColonizeOnWithoutColonists()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();
        var context = turnManager.CreateTurnContext(p1);

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = Guid.Empty;
        location.Trait<Populatable>().Population = 0;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        ship.Trait<ColonizationCapable>().IsLoaded = false;
        location.Trait<Hospitality>().Enter(ship);

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(Guid.Empty, location.Trait<Associatable>().PlayerId);
        Assert.Equal(0, location.Trait<Populatable>().Population);
        Assert.False(ship.Trait<ColonizationCapable>().IsLoaded);
    }

    [Fact]
    public async Task ColonizeTranscript_Turn_RecruitColonists()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();
        var context = turnManager.CreateTurnContext(p1);

        var location = new Planet();
        location.Trait<Associatable>().PlayerId = p1;
        location.Trait<Populatable>().Population = 1234;
        context.ActorContext.AddActor(location);

        var ship = context.ActorContext.AddShip<ColonizeShip>(p1);
        ship.Trait<ColonizationCapable>().IsLoaded = false;
        location.Trait<Hospitality>().Enter(ship);

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(p1, location.Trait<Associatable>().PlayerId);
        Assert.Equal(1234, location.Trait<Populatable>().Population);
        Assert.True(ship.Trait<ColonizationCapable>().IsLoaded);
    }

    public async Task<(TurnManager turnManager, Guid p1, Guid p2, Guid p3)> CreateEnvironmentAsync()
    {
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        var p3 = Guid.NewGuid();

        var turnManager = await new TurnManagerBuilder(new ServiceCollection()
            .Configure<ResearchOptions>(_ => { })
            .BuildServiceProvider())
            .AddPlayer(CreateHumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1))
            .AddPlayer(CreateHumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1))
            .AddPlayer(CreateHumanPlayer(p3, "orange", new Guid[] { }, 20, 1))
            .AddElementsTranscript()
            .BuildAsync();

        return (turnManager, p1, p2, p3);
    }
}
