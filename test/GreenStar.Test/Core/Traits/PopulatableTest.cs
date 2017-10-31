using System;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Players;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class PopulatableTest
    {
        [Fact]
        public void Populatable_Life_PopulationGrowNotWithoutPopulation()
        {
            // arrange
            var (game, p1) = CreateEnvironment();

            var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
            var populatable = new Populatable(associatable) { Population = 0 };

            // act
            populatable.Life(game);

            // assert
            Assert.Equal(0, populatable.Population);
        }

        [Fact]
        public void Populatable_Life_PopulationGrowWithoutIdealConditionOn1000()
        {
            // arrange
            var (game, p1) = CreateEnvironment();

            var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
            var populatable = new Populatable(associatable) { Population = 1000, SurfaceTemperature = 22, Gravity = 1.0, MiningPercentage = 0 };

            // act
            populatable.Life(game);

            // assert
            Assert.Equal(1500, populatable.Population);
        }

        [Fact]
        public void Populatable_Life_PopulationGrowWithUpperLimit()
        {
            // arrange
            var (game, p1) = CreateEnvironment();

            var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
            var populatable = new Populatable(associatable) { Population = 4_500_000_000, SurfaceTemperature = 22, Gravity = 1.0, MiningPercentage = 0 };

            // act
            populatable.Life(game);

            // assert
            Assert.Equal(4_999_990_000, populatable.Population);
        }
        
        [Fact]
        public void Populatable_Life_TerraformNotWithoutPopulation()
        {
            // arrange
            var (game, p1) = CreateEnvironment();

            var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
            var populatable = new Populatable(associatable) { Population = 0, SurfaceTemperature = 100, Gravity = 1.0 };

            // act
            populatable.Life(game);

            // assert
            Assert.Equal(100, populatable.SurfaceTemperature);
        }

        [Fact]
        public void Populatable_Life_TerraformWithPopulationFrom100()
        {
            // arrange
            var (game, p1) = CreateEnvironment();

            var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
            var populatable = new Populatable(associatable) { Population = 1000, SurfaceTemperature = 100, Gravity = 1.0, MiningPercentage = 0 };

            // act
            populatable.Life(game);

            // assert
            Assert.Equal(99, populatable.SurfaceTemperature);
        }


        public (IPlayerContext game, Guid p1) CreateEnvironment()
        {
            Guid p1 = Guid.NewGuid();

            var turnEngine = new TurnManagerBuilder()
                .AddPlayer(new HumanPlayer(p1, "red", new Guid[0], 22, 1.0))
                .Build();

            return (turnEngine.Game, p1);
        }
    }
}