using System;
using System.Linq;
using GreenStar.Core.Cartography;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine.Players;
using GreenStar.Ships;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class VectorFlightTranscriptTest
    {
        [Fact]
        public void VectorFlightTranscript_Execute_NoMovement()
        {
            // arrange
            CreateEnvironment(out var p1Guid, out var l1, out var l2, out var scout, out var turnEngine);

            // act
            turnEngine.FinishTurn(p1Guid);

            // assert
            Assert.Equal(1, turnEngine.Turn);
            Assert.False(scout.Trait<LocatableTrait>().HasOwnPosition);
            Assert.Equal(l1.Id, scout.Trait<LocatableTrait>().HostLocationActorId);
        }

        [Fact]
        public void VectorFlightTranscript_Execute_StartFlight()
        {
            // arrange
            CreateEnvironment(out var p1Guid, out var l1, out var l2, out var scout, out var turnEngine);
            scout.Trait<VectorFlightCapableTrait>().StartFlight(turnEngine.Game, l2);

            // act
            turnEngine.FinishTurn(p1Guid);

            // assert
            Assert.Equal(1, turnEngine.Turn);
            Assert.Equal(3, turnEngine.Game.Actors.Count());
            Assert.Equal(5, scout.Trait<VectorFlightCapableTrait>().Fuel);
            Assert.True(scout.Trait<LocatableTrait>().HasOwnPosition);
            Assert.Equal(Guid.Empty, scout.Trait<LocatableTrait>().HostLocationActorId);
            Assert.Equal(1000, scout.Trait<LocatableTrait>().Position.X);
            Assert.Equal(1500, scout.Trait<LocatableTrait>().Position.Y);
        }

        [Fact]
        public void VectorFlightTranscript_Execute_Arrived()
        {
            // arrange
            CreateEnvironment(out var p1Guid, out var l1, out var l2, out var scout, out var turnEngine);
            scout.Trait<VectorFlightCapableTrait>().StartFlight(turnEngine.Game, l2);

            // act
            turnEngine.FinishTurn(p1Guid);
            turnEngine.FinishTurn(p1Guid);

            // assert
            Assert.Equal(2, turnEngine.Turn);
            Assert.Equal(3, turnEngine.Game.Actors.Count());
            Assert.Equal(0, scout.Trait<VectorFlightCapableTrait>().Fuel);
            Assert.False(scout.Trait<LocatableTrait>().HasOwnPosition);
            Assert.Equal(l2.Id, scout.Trait<LocatableTrait>().HostLocationActorId);
            Assert.Equal(1000, scout.Trait<LocatableTrait>().Position.X);
            Assert.Equal(2000, scout.Trait<LocatableTrait>().Position.Y);
        }

        
        [Fact]
        public void VectorFlightTranscript_Execute_NoFuel()
        {
            // arrange
            CreateEnvironment(out var p1Guid, out var l1, out var l2, out var scout, out var turnEngine);
            scout.Trait<VectorFlightCapableTrait>().StartFlight(turnEngine.Game, l2);
            scout.Trait<VectorFlightCapableTrait>().Fuel = 7;

            // act
            turnEngine.FinishTurn(p1Guid);
            turnEngine.FinishTurn(p1Guid);

            // assert
            Assert.Equal(2, turnEngine.Turn);
            Assert.Equal(4, turnEngine.Game.Actors.Count());
            Assert.Equal(0, scout.Trait<VectorFlightCapableTrait>().Fuel);
            Assert.False(scout.Trait<LocatableTrait>().HasOwnPosition);

            var newPositionActorId = scout.Trait<LocatableTrait>().HostLocationActorId;
            var newPositionActor = turnEngine.Game.GetActor(newPositionActorId);
            Assert.Equal(1000, newPositionActor.Trait<LocatableTrait>().Position.X);
            Assert.Equal(1700, newPositionActor.Trait<LocatableTrait>().Position.Y);
        }

        private static void CreateEnvironment(out Guid p1Guid, out ExactLocation l1, out ExactLocation l2, out Scout scout, out Core.TurnEngine.TurnManager turnEngine)
        {
            p1Guid = Guid.NewGuid();
            var player = new HumanPlayer(p1Guid, "Red", new Guid[0]);

            l1 = new ExactLocation(Guid.NewGuid());
            l1.Trait<LocatableTrait>().Position = new Coordinate(1000, 1000);

            l2 = new ExactLocation(Guid.NewGuid());
            l2.Trait<LocatableTrait>().Position = new Coordinate(1000, 2000);
            
            scout = new Scout(Guid.NewGuid());
            scout.Trait<VectorFlightCapableTrait>().Fuel = 10;

            l1.Trait<HostTrait>().Enter(scout);

            turnEngine = new TurnManagerBuilder()
                .AddActor(scout)
                .AddActor(l1)
                .AddActor(l2)
                .AddTranscript(new VectorFlightTranscript())
                .AddPlayer(player)
                .Build();
        }
    }
}