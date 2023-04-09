using System;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.Resources;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using static GreenStar.Test.Helper;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Transcripts;

public class CalculateResourceRevenuesTest
{
    [Theory]
    [InlineData(1_000, 1_000, -25, 100)]
    [InlineData(100_000, 1_000, 7_400, 100)]
    public async Task CalculateResourceRevenues_Turn_SingleOccupiedPlanet(int population, int metalStock, int expectedMoney, int expectedMetal)
    {
        // assert
        var (turnManager, player1, player2, player3, planet1, planet2) = await CreateEnvironmentAsync();

        planet1.Trait<Associatable>().PlayerId = player1.Id;
        planet1.Trait<Nameable>().Name = "Foo";
        planet1.Trait<Populatable>().Gravity = 1;
        planet1.Trait<Populatable>().SurfaceTemperature = 20;
        planet1.Trait<Populatable>().MiningPercentage = 100;
        planet1.Trait<Populatable>().Population = population;
        planet1.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(expectedMoney, player1.Resourceful.Resources[ResourceConstants.Money]);
        Assert.Equal(expectedMetal, player1.Resourceful.Resources[ResourceConstants.Metal]);
    }


    [Theory]
    [InlineData(100_000, 1_000, 7_400, 100)]
    public async Task CalculateResourceRevenues_Turn_TwoOccupiedPlanet(int population, int metalStock, int expectedMoney, int expectedMetal)
    {
        // assert
        var (turnManager, player1, player2, player3, planet1, planet2) = await CreateEnvironmentAsync();

        planet1.Trait<Associatable>().PlayerId = player1.Id;
        planet1.Trait<Nameable>().Name = "Foo";
        planet1.Trait<Populatable>().Gravity = 1;
        planet1.Trait<Populatable>().SurfaceTemperature = 20;
        planet1.Trait<Populatable>().MiningPercentage = 100;
        planet1.Trait<Populatable>().Population = population;
        planet1.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

        planet2.Trait<Associatable>().PlayerId = player1.Id;
        planet2.Trait<Nameable>().Name = "Bar";
        planet2.Trait<Populatable>().Gravity = 1;
        planet2.Trait<Populatable>().SurfaceTemperature = 20;
        planet2.Trait<Populatable>().MiningPercentage = 100;
        planet2.Trait<Populatable>().Population = population;
        planet2.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(2 * expectedMoney, player1.Resourceful.Resources[ResourceConstants.Money]);
        Assert.Equal(2 * expectedMetal, player1.Resourceful.Resources[ResourceConstants.Metal]);
    }


    [Theory]
    [InlineData(100_000, 1_000, 7_400, 100)]
    public async Task CalculateResourceRevenues_Turn_OneOccupiedPlanetOneEnemyPlanet(int population, int metalStock, int expectedMoney, int expectedMetal)
    {
        // assert
        var (turnManager, player1, player2, player3, planet1, planet2) = await CreateEnvironmentAsync();

        planet1.Trait<Associatable>().PlayerId = player1.Id;
        planet1.Trait<Nameable>().Name = "Foo";
        planet1.Trait<Populatable>().Gravity = 1;
        planet1.Trait<Populatable>().SurfaceTemperature = 20;
        planet1.Trait<Populatable>().MiningPercentage = 100;
        planet1.Trait<Populatable>().Population = population;
        planet1.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

        planet2.Trait<Associatable>().PlayerId = player2.Id;
        planet2.Trait<Nameable>().Name = "Bar";
        planet2.Trait<Populatable>().Gravity = 1;
        planet2.Trait<Populatable>().SurfaceTemperature = 20;
        planet2.Trait<Populatable>().MiningPercentage = 100;
        planet2.Trait<Populatable>().Population = population;
        planet2.Trait<Resourceful>().Resources = new ResourceAmount(new ResourceAmountItem(ResourceConstants.Metal, metalStock));

        // act
        await turnManager.FinishTurnForAllPlayersAsync();

        // assert
        Assert.Equal(expectedMoney, player1.Resourceful.Resources[ResourceConstants.Money]);
        Assert.Equal(expectedMetal, player1.Resourceful.Resources[ResourceConstants.Metal]);
    }

    public async Task<(TurnManager turnManager, Player player1, Player player2, Player player3, Planet planet1, Planet planet2)> CreateEnvironmentAsync()
    {
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        var p3 = Guid.NewGuid();
        var player1 = CreateHumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1);
        var player2 = CreateHumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1);
        var player3 = CreateHumanPlayer(p3, "orange", new Guid[] { }, 20, 1);
        var planet1 = new Planet();
        var planet2 = new Planet();

        var turnManager = await new TurnManagerBuilder(new ServiceCollection()
            .Configure<PlanetLifeOptions>(_ => { })
            .BuildServiceProvider())
            .AddPlayer(player1)
            .AddPlayer(player2)
            .AddPlayer(player3)
            .AddActor(planet1)
            .AddActor(planet2)
            .AddCoreTranscript()
            .AddStellarTranscript()
            .BuildAsync();

        return (turnManager, player1, player2, player3, planet1, planet2);
    }
}
