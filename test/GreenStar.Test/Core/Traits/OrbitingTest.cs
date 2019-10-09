using System;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Players;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class OrbitingTest
    {
        [Theory]
        [InlineData(0, 100, 0)]
        [InlineData(1, 0, 100)]
        [InlineData(2, -100, 0)]
        [InlineData(3, 0, -100)]
        [InlineData(4, 100, 0)]
        [InlineData(5, 0, 100)]
        public void Orbiting_Simple(int turns, long x, long y)
        {
            // arrange
            var sun = new Sun();
            sun.Trait<Locatable>().Position = (0, 0);
            var planet = new Planet();
            var orbiting = planet.Trait<Orbiting>();
            orbiting.Host = sun.Id;
            orbiting.Distance = 100;
            orbiting.SpeedDegree = 90;
            orbiting.CurrentDegree = 0;
            planet.Trait<Locatable>().Position = (100, 0);

            var turnManager = new TurnManagerBuilder()
                .AddStellarTranscript()
                .AddActor(sun)
                .AddActor(planet)
                .Build();

            // act
            for (int i = 0; i < turns; i++)
            {
                turnManager.StartRound();
            }

            // assert
            Assert.Equal((x, y), planet.Trait<Locatable>().Position);
            Assert.Equal((turns * 90) % 360, orbiting.CurrentDegree);
        }
    }
}