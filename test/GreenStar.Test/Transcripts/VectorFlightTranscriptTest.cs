using System;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.Cartography;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit;

namespace GreenStar.Transcripts;

public class VectorFlightTranscriptTest
{
    [Fact]
    public async Task VectorFlightTranscript_Execute_NoMovement()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);

        // act
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public async Task VectorFlightTranscript_Execute_StartStopFlight()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);

        // act
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2, new ResearchOptions());
        scout.Trait<VectorFlightCapable>().StopFlight(context.ActorContext, context.TurnContext);

        // assert
        Assert.Equal(0, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public async Task VectorFlightTranscript_Execute_StartFlight()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2, new ResearchOptions());

        // act
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.Equal(5, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.True(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(Guid.Empty, scout.Trait<Locatable>().HostLocationActorId);
        Assert.Equal((1000, 1500), scout.Trait<Locatable>().GetPosition(context.ActorContext));
    }

    [Fact]
    public async Task VectorFlightTranscript_Execute_StartFlightWithCommand()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        var commands = scout.GetCommands();

        var orderMoveCommand = commands.OfType<OrderMoveCommand>().FirstOrDefault();
        orderMoveCommand.Arguments[0] = orderMoveCommand.Arguments[0] with
        {
            Value = l2.Id.ToString(),
        };

        await context.TurnContext.ExecuteCommandAsync(context, player, orderMoveCommand);

        // act
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.Equal(5, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.True(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(Guid.Empty, scout.Trait<Locatable>().HostLocationActorId);
        Assert.Equal((1000, 1500), scout.Trait<Locatable>().GetPosition(context.ActorContext));
    }


    [Fact]
    public async Task VectorFlightTranscript_Execute_StartFlightRejectedSourceTargetSameId()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        var result = scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l1, new ResearchOptions());

        // act
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.False(result);
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public async Task VectorFlightTranscript_Execute_StartFlightRejectedSourceTargetSameLocation()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        var result = scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l3, new ResearchOptions());

        // act
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.False(result);
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public async Task VectorFlightTranscript_Execute_Arrived()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2, new ResearchOptions());

        // act
        await turnEngine.FinishTurnAsync(p1Guid);
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.Equal(2, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.Equal(0, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l2.Id, scout.Trait<Locatable>().HostLocationActorId);
        Assert.Equal((1000, 2000), scout.Trait<Locatable>().GetPosition(context.ActorContext));
    }


    [Fact]
    public async Task VectorFlightTranscript_Execute_NoFuel()
    {
        // arrange
        var (p1Guid, l1, l2, l3, scout, player, turnEngine) = await CreateEnvironmentAsync();
        var context = turnEngine.CreateTurnContext(p1Guid);
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2, new ResearchOptions());
        scout.Trait<VectorFlightCapable>().Fuel = 7;

        // act
        await turnEngine.FinishTurnAsync(p1Guid);
        await turnEngine.FinishTurnAsync(p1Guid);

        // assert
        Assert.Equal(2, context.TurnContext.Turn);
        Assert.Equal(5, context.ActorContext.AsQueryable().Count());
        Assert.Equal(0, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);

        var newPositionActorId = scout.Trait<Locatable>().HostLocationActorId;
        var newPositionActor = context.ActorContext.GetActor(newPositionActorId);
        Assert.Equal((1000, 1700), newPositionActor.Trait<Locatable>().GetPosition(context.ActorContext));

        var newExactLocation = context.ActorContext.AsQueryable().Where(x => x is ExactLocation).Where(x => x != l1 && x != l2 && x != l3).FirstOrDefault();
        Assert.NotNull(newExactLocation);
        Assert.Equal((1000, 1700), newExactLocation.Trait<Locatable>().GetPosition(context.ActorContext));
        Assert.True(newExactLocation.Trait<Discoverable>().IsDiscoveredBy(p1Guid, DiscoveryLevel.PropertyAware));

    }

    private static async Task<(Guid p1Guid, ExactLocation l1, ExactLocation l2, ExactLocation l3, Scout scout, Player player, TurnManager turnEngine)> CreateEnvironmentAsync()
    {
        var p1Guid = Guid.NewGuid();
        var player = new HumanPlayer(p1Guid, "Red", new Guid[0], 22, 1);

        var l1 = new ExactLocation();
        l1.Trait<Locatable>().SetPosition((1000, 1000));

        var l2 = new ExactLocation();
        l2.Trait<Locatable>().SetPosition((1000, 2000));

        var l3 = new ExactLocation();
        l3.Trait<Locatable>().SetPosition((1000, 1000));

        var scout = new Scout();
        scout.Trait<VectorFlightCapable>().Fuel = 10;
        scout.Trait<Capable>().Of(ShipCapabilities.Speed, 5);

        l1.Trait<Hospitality>().Enter(scout);

        var turnEngine = await new TurnManagerBuilder(new ServiceCollection()
            .Configure<ResearchOptions>(_ => { })
            .BuildServiceProvider())
            .AddActor(scout)
            .AddActor(l1)
            .AddActor(l2)
            .AddActor(l3)
            .AddTranscript(TurnTranscriptGroups.Moves, new VectorFlightTurnTranscript(Options.Create(new ResearchOptions())))
            .AddPlayer(player)
            .BuildAsync();

        return (p1Guid, l1, l2, l3, scout, player, turnEngine);
    }
}
