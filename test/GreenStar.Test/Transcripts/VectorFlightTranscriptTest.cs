using System;
using System.Linq;

using GreenStar.Cartography;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Xunit;

namespace GreenStar.Transcripts;

public class VectorFlightTranscriptTest
{
    [Fact]
    public void VectorFlightTranscript_Execute_NoMovement()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out _, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);

        // act
        turnEngine.FinishTurn(p1Guid);

        // assert
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public void VectorFlightTranscript_Execute_StartStopFlight()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out _, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);

        // act
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2);
        scout.Trait<VectorFlightCapable>().StopFlight(context.ActorContext, context.TurnContext);

        // assert
        Assert.Equal(0, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public void VectorFlightTranscript_Execute_StartFlight()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out _, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2);

        // act
        turnEngine.FinishTurn(p1Guid);

        // assert
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.Equal(5, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.True(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(Guid.Empty, scout.Trait<Locatable>().HostLocationActorId);
        Assert.Equal((1000, 1500), scout.Trait<Locatable>().GetPosition(context.ActorContext));
    }

    [Fact]
    public void VectorFlightTranscript_Execute_StartFlightWithCommand()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out _, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        var commands = scout.GetCommands();

        var orderMoveCommand = commands.OfType<OrderMoveCommand>().FirstOrDefault();
        orderMoveCommand.Arguments[0] = orderMoveCommand.Arguments[0] with
        {
            Value = l2.Id.ToString(),
        };

        context.TurnContext.ExecuteCommand(context, player, orderMoveCommand);

        // act
        turnEngine.FinishTurn(p1Guid);

        // assert
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.Equal(5, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.True(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(Guid.Empty, scout.Trait<Locatable>().HostLocationActorId);
        Assert.Equal((1000, 1500), scout.Trait<Locatable>().GetPosition(context.ActorContext));
    }


    [Fact]
    public void VectorFlightTranscript_Execute_StartFlightRejectedSourceTargetSameId()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out _, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        var result = scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l1);

        // act
        turnEngine.FinishTurn(p1Guid);

        // assert
        Assert.False(result);
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public void VectorFlightTranscript_Execute_StartFlightRejectedSourceTargetSameLocation()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out var l3, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        var result = scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l3);

        // act
        turnEngine.FinishTurn(p1Guid);

        // assert
        Assert.False(result);
        Assert.Equal(1, context.TurnContext.Turn);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l1.Id, scout.Trait<Locatable>().HostLocationActorId);
    }

    [Fact]
    public void VectorFlightTranscript_Execute_Arrived()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out _, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2);

        // act
        turnEngine.FinishTurn(p1Guid);
        turnEngine.FinishTurn(p1Guid);

        // assert
        Assert.Equal(2, context.TurnContext.Turn);
        Assert.Equal(4, context.ActorContext.AsQueryable().Count());
        Assert.Equal(0, scout.Trait<VectorFlightCapable>().Fuel);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(l2.Id, scout.Trait<Locatable>().HostLocationActorId);
        Assert.Equal((1000, 2000), scout.Trait<Locatable>().GetPosition(context.ActorContext));
    }


    [Fact]
    public void VectorFlightTranscript_Execute_NoFuel()
    {
        // arrange
        CreateEnvironment(out var p1Guid, out var l1, out var l2, out var l3, out var scout, out var player, out var turnEngine);
        var context = turnEngine.CreateTurnContext(p1Guid);
        scout.Trait<VectorFlightCapable>().StartFlight(context.ActorContext, l2);
        scout.Trait<VectorFlightCapable>().Fuel = 7;

        // act
        turnEngine.FinishTurn(p1Guid);
        turnEngine.FinishTurn(p1Guid);

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

    private static void CreateEnvironment(out Guid p1Guid, out ExactLocation l1, out ExactLocation l2, out ExactLocation l3, out Scout scout, out Player player, out TurnManager turnEngine)
    {
        p1Guid = Guid.NewGuid();
        player = new HumanPlayer(p1Guid, "Red", new Guid[0], 22, 1);

        l1 = new ExactLocation();
        l1.Trait<Locatable>().SetPosition((1000, 1000));

        l2 = new ExactLocation();
        l2.Trait<Locatable>().SetPosition((1000, 2000));

        l3 = new ExactLocation();
        l3.Trait<Locatable>().SetPosition((1000, 1000));

        scout = new Scout();
        scout.Trait<VectorFlightCapable>().Fuel = 10;
        scout.Trait<Capable>().Of(ShipCapabilities.Speed, 5);

        l1.Trait<Hospitality>().Enter(scout);

        turnEngine = new TurnManagerBuilder()
            .AddActor(scout)
            .AddActor(l1)
            .AddActor(l2)
            .AddActor(l3)
            .AddTranscript(TurnTranscriptGroups.Moves, new VectorFlightTurnTranscript())
            .AddPlayer(player)
            .Build();
    }
}
