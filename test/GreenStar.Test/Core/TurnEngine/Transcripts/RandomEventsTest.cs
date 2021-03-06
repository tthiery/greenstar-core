using System;
using System.Linq;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Players;
using GreenStar.Events;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class RandomEventsTest
    {
        [Fact]
        public void ChangePopulationEventExecutor_Execute_Normal()
        {
            // arrange
            var (turnManager, p1, p2, p3) = CreateEnvironment();

            var transcript = turnManager.Transcripts.OfType<RandomEvents>().FirstOrDefault();
            var context = turnManager.CreateTurnContext();

            var planet = new Planet();
            planet.Trait<Populatable>().Population = 100;
            context.ActorContext.AddActor(planet);
            planet.Trait<Associatable>().PlayerId = p1;

            // act
            transcript.ApplyEventToPlayer(context, new RandomEvent()
            {
                Name = nameof(ChangePopulationEventExecutor_Execute_Normal),
                Type = "ChangePopulation",
                Argument = "-0,4",
                IsReturning = true,
                Prohability = 100,
                Text = "A riot occured on {0}. You lost {1} of your population.",
            }, context.PlayerContext.GetPlayer(p1));

            // assert
            Assert.Equal(60, planet.Trait<Populatable>().Population);
        }

        [Fact]
        public void RandomEvents_ApplyEventToPlayer_InvokeCorrectEventExecutorWithCorrectPlayerIdAndArgument()
            => RandomEvents_ApplyEventToPlayer_FilterByCapability(new string[] { }, null, null, true);

        [Theory]
        [InlineData(new string[] { }, null, null, true)] // no filters set
        [InlineData(new string[] { "tech-foo" }, new string[] { "tech-foo" }, new string[] { }, true)] // a required technology is available
        [InlineData(new string[] { "tech-bar" }, new string[] { "tech-foo" }, new string[] { }, false)] // a required technology is available
        [InlineData(new string[] { "tech-bar" }, new string[] { "tech-foo", "tech-bar" }, new string[] { }, false)] // some of the required technology is not available
        [InlineData(new string[] { "tech-foo", "tech-bar" }, new string[] { "tech-foo", "tech-bar" }, new string[] { }, true)] // all of the required technology is available
        [InlineData(new string[] { "tech-foo" }, new string[] { }, new string[] { "tech-foo" }, false)] // a blocking technology is present
        [InlineData(new string[] { "tech-foo" }, new string[] { }, new string[] { "tech-bar" }, true)] // a blocking technology is not present
        [InlineData(new string[] { "tech-foo" }, new string[] { }, new string[] { "tech-foo", "tech-bar" }, false)] // a blocking technology is present
        public void RandomEvents_ApplyEventToPlayer_FilterByCapability(string[] playerCapabilities, string[] requiredCapabilities, string[] blockingCapabilities, bool executed)
        {
            // arrange
            var (turnManager, p1, p2, p3) = CreateEnvironment();

            var transcript = turnManager.Transcripts.OfType<RandomEvents>().FirstOrDefault();
            var context = turnManager.CreateTurnContext();

            var planet = new Planet();
            planet.Trait<Populatable>().Population = 100;
            context.ActorContext.AddActor(planet);
            planet.Trait<Associatable>().PlayerId = p1;

            var player = context.PlayerContext.GetPlayer(p1);
            foreach (var playerCapability in playerCapabilities)
            {
                player.Capable.AddCapability(playerCapability);
                player.Capable.Of(playerCapability, 1);
            }

            // act
            transcript.ApplyEventToPlayer(context, new RandomEvent()
            {
                Name = nameof(RandomEvents_ApplyEventToPlayer_FilterByCapability),
                Type = "ChangePopulation",
                Argument = "-0,4",
                IsReturning = true,
                Prohability = 100,
                RequiredTechnologies = requiredCapabilities,
                BlockingTechnologies = blockingCapabilities,
                Text = "A riot occured on {0}. You lost {1} of your population.",
            }, context.PlayerContext.GetPlayer(p1));

            // assert
            Assert.Equal(executed ? 60 : 100, planet.Trait<Populatable>().Population);
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
                .AddEventTranscripts()
                .Build();

            return (turnManager, p1, p2, p3);
        }
    }
}