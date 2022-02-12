using System;
using System.Threading.Tasks;

using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Transcripts;

public class ResourceGiftEvenTest
{
    [Fact]
    public async Task ResourceGiftEvent_Execute_Normal()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();

        var context = turnManager.CreateTurnContext(p1);
        var player1 = context.PlayerContext.GetPlayer(p1);
        player1.Resources = "Metal:100,Money:200";

        // act
        await context.TurnContext.ExecuteEventAsync(context, player1, "GreenStar.Transcripts.ResourceGiftEvent, GreenStar.Events", new[] { "Metal:+10, Money:+20" }, "You found resources");

        // assert
        Assert.Equal(110, player1.Resources["Metal"]);
        Assert.Equal(220, player1.Resources["Money"]);
    }
    [Fact]
    public async Task ResourceGiftEvent_Execute_Minus()
    {
        // arrange
        var (turnManager, p1, p2, p3) = await CreateEnvironmentAsync();

        var context = turnManager.CreateTurnContext(p1);
        var player1 = context.PlayerContext.GetPlayer(p1);
        player1.Resources = "Metal:100,Money:200";

        // act
        await context.TurnContext.ExecuteEventAsync(context, player1, "GreenStar.Transcripts.ResourceGiftEvent, GreenStar.Events", new[] { "Metal:+10, Money:-20" }, "You found resources");

        // assert
        Assert.Equal(110, player1.Resources["Metal"]);
        Assert.Equal(180, player1.Resources["Money"]);
    }

    public async Task<(TurnManager turnManager, Guid p1, Guid p2, Guid p3)> CreateEnvironmentAsync()
    {
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        var p3 = Guid.NewGuid();

        var turnManager = await new TurnManagerBuilder(new ServiceCollection()
            .BuildServiceProvider())
            .AddPlayer(new HumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1))
            .AddPlayer(new HumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1))
            .AddPlayer(new HumanPlayer(p3, "orange", new Guid[] { }, 20, 1))
            .BuildAsync();

        return (turnManager, p1, p2, p3);
    }
}