using System;
using System.Linq;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Players;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Cartography.Builder
{
    public class SolSystemBuilderTest
    {
        [Fact]
        public void SolarSystemBuilder_Simple()
        {
            // arrange
            var turnManagerBuilder = new TurnManagerBuilder();

            // act
            var turnManager = turnManagerBuilder
                .AddActors(new SolSystemBuilder())
                .Build();
            var context = turnManager.CreateTurnContext();

            // assert
            Assert.Equal(9, context.ActorContext.AsQueryable().Count());
            Assert.Equal(1, context.ActorContext.AsQueryable().OfType<Sun>().Count());
            Assert.Equal(8, context.ActorContext.AsQueryable().OfType<Planet>().Count());
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
                .AddActors(new SolSystemBuilder())
                .Build();

            return (turnManager, p1, p2, p3);
        }
    }
}