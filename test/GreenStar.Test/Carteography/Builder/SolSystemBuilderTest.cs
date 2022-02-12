using System;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.Stellar;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Cartography.Builder;

public class SolSystemBuilderTest
{
    [Fact]
    public async Task SolarSystemBuilder_Simple()
    {
        // arrange
        var turnManagerBuilder = new TurnManagerBuilder(new ServiceCollection().BuildServiceProvider());

        // act
        var turnManager = await turnManagerBuilder
            .AddActors(new SolSystemBuilder())
            .BuildAsync();
        var context = turnManager.CreateTurnContext(SystemPlayer.SystemPlayerId);

        // assert
        Assert.Equal(9, context.ActorContext.AsQueryable().Count());
        Assert.Equal(1, context.ActorContext.AsQueryable().OfType<Sun>().Count());
        Assert.Equal(8, context.ActorContext.AsQueryable().OfType<Planet>().Count());
    }


    public async Task<(TurnManager turnManager, Guid p1, Guid p2, Guid p3)> CreateEnvironment()
    {
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        var p3 = Guid.NewGuid();

        var turnManager = await new TurnManagerBuilder(new ServiceCollection().BuildServiceProvider())
            .AddPlayer(new HumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1))
            .AddPlayer(new HumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1))
            .AddPlayer(new HumanPlayer(p3, "orange", new Guid[] { }, 20, 1))
            .AddActors(new SolSystemBuilder())
            .BuildAsync();

        return (turnManager, p1, p2, p3);
    }
}
