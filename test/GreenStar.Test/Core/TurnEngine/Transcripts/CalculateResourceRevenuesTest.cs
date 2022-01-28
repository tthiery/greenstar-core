using System;
using GreenStar.Core.Resources;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine.Players;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class CalculateResourceRevenuesTest
    {
        [Theory]
        [InlineData(1_000, 1_000, -25, 100)]
        [InlineData(100_000, 1_000, 7_400, 100)]
        public void CalculateResourceRevenues_Turn_SingleOccupiedPlanet(int population, int metalStock, int expectedMoney, int expectedMetal)
        {
            // assert
            var (turnManager, player1, player2, player3, planet1, planet2) = CreateEnvironment();

            planet1.Trait<Associatable>().PlayerId = player1.Id;
            planet1.Trait<Associatable>().Name = "Foo";
            planet1.Trait<Populatable>().Gravity = 1;
            planet1.Trait<Populatable>().SurfaceTemperature = 20;
            planet1.Trait<Populatable>().MiningPercentage = 100;
            planet1.Trait<Populatable>().Population = population;
            planet1.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

            // act
            turnManager.FinishTurnForAllPlayers();

            // assert
            Assert.Equal(expectedMoney, player1.Resources[ResourceConstants.Money]);
            Assert.Equal(expectedMetal, player1.Resources[ResourceConstants.Metal]);
        }


        [Theory]
        [InlineData(100_000, 1_000, 7_400, 100)]
        public void CalculateResourceRevenues_Turn_TwoOccupiedPlanet(int population, int metalStock, int expectedMoney, int expectedMetal)
        {
            // assert
            var (turnManager, player1, player2, player3, planet1, planet2) = CreateEnvironment();

            planet1.Trait<Associatable>().PlayerId = player1.Id;
            planet1.Trait<Associatable>().Name = "Foo";
            planet1.Trait<Populatable>().Gravity = 1;
            planet1.Trait<Populatable>().SurfaceTemperature = 20;
            planet1.Trait<Populatable>().MiningPercentage = 100;
            planet1.Trait<Populatable>().Population = population;
            planet1.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

            planet2.Trait<Associatable>().PlayerId = player1.Id;
            planet2.Trait<Associatable>().Name = "Bar";
            planet2.Trait<Populatable>().Gravity = 1;
            planet2.Trait<Populatable>().SurfaceTemperature = 20;
            planet2.Trait<Populatable>().MiningPercentage = 100;
            planet2.Trait<Populatable>().Population = population;
            planet2.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

            // act
            turnManager.FinishTurnForAllPlayers();

            // assert
            Assert.Equal(2 * expectedMoney, player1.Resources[ResourceConstants.Money]);
            Assert.Equal(2 * expectedMetal, player1.Resources[ResourceConstants.Metal]);
        }


        [Theory]
        [InlineData(100_000, 1_000, 7_400, 100)]
        public void CalculateResourceRevenues_Turn_OneOccupiedPlanetOneEnemyPlanet(int population, int metalStock, int expectedMoney, int expectedMetal)
        {
            // assert
            var (turnManager, player1, player2, player3, planet1, planet2) = CreateEnvironment();

            planet1.Trait<Associatable>().PlayerId = player1.Id;
            planet1.Trait<Associatable>().Name = "Foo";
            planet1.Trait<Populatable>().Gravity = 1;
            planet1.Trait<Populatable>().SurfaceTemperature = 20;
            planet1.Trait<Populatable>().MiningPercentage = 100;
            planet1.Trait<Populatable>().Population = population;
            planet1.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

            planet2.Trait<Associatable>().PlayerId = player2.Id;
            planet2.Trait<Associatable>().Name = "Bar";
            planet2.Trait<Populatable>().Gravity = 1;
            planet2.Trait<Populatable>().SurfaceTemperature = 20;
            planet2.Trait<Populatable>().MiningPercentage = 100;
            planet2.Trait<Populatable>().Population = population;
            planet2.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

            // act
            turnManager.FinishTurnForAllPlayers();

            // assert
            Assert.Equal(expectedMoney, player1.Resources[ResourceConstants.Money]);
            Assert.Equal(expectedMetal, player1.Resources[ResourceConstants.Metal]);
        }

        public (TurnManager turnManager, Player player1, Player player2, Player player3, Planet planet1, Planet planet2) CreateEnvironment()
        {
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var player1 = new HumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1);
            var player2 = new HumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1);
            var player3 = new HumanPlayer(p3, "orange", new Guid[] { }, 20, 1);
            var planet1 = new Planet();
            var planet2 = new Planet();

            var turnManager = new TurnManagerBuilder()
                .AddPlayer(player1)
                .AddPlayer(player2)
                .AddPlayer(player3)
                .AddActor(planet1)
                .AddActor(planet2)
                .AddCoreTranscript()
                .AddStellarTranscript()
                .Build();

            return (turnManager, player1, player2, player3, planet1, planet2);
        }
    }
}