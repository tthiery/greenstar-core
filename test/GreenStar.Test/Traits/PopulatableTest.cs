using System;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Traits;

public class PopulatableTest
{
    [Fact]
    public async Task Populatable_Life_PopulationGrowNotWithoutPopulation()
    {
        // arrange
        var (context, p1) = await CreateEnvironmentAsync();

        var associatable = new Associatable() { PlayerId = p1 };
        var nameable = new Nameable() { Name = "XYZ" };
        var populatable = new Populatable(nameable, associatable) { Population = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(0, populatable.Population);
    }

    [Fact]
    public async Task Populatable_Life_PopulationGrowWithoutIdealConditionOn1000()
    {
        // arrange
        var (context, p1) = await CreateEnvironmentAsync();

        var associatable = new Associatable() { PlayerId = p1 };
        var nameable = new Nameable() { Name = "XYZ" };
        var populatable = new Populatable(nameable, associatable) { Population = 1000, SurfaceTemperature = 22, Gravity = 1.0, MiningPercentage = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(1500, populatable.Population);
    }

    [Fact]
    public async Task Populatable_Life_PopulationGrowWithUpperLimit()
    {
        // arrange
        var (context, p1) = await CreateEnvironmentAsync();

        var associatable = new Associatable() { PlayerId = p1 };
        var nameable = new Nameable() { Name = "XYZ" };
        var populatable = new Populatable(nameable, associatable) { Population = 4_500_000_000, SurfaceTemperature = 22, Gravity = 1.0, MiningPercentage = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(4_999_990_000, populatable.Population);
    }

    [Fact]
    public async Task Populatable_Life_TerraformNotWithoutPopulation()
    {
        // arrange
        var (context, p1) = await CreateEnvironmentAsync();

        var associatable = new Associatable() { PlayerId = p1 };
        var nameable = new Nameable() { Name = "XYZ" };
        var populatable = new Populatable(nameable, associatable) { Population = 0, SurfaceTemperature = 100, Gravity = 1.0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(100, populatable.SurfaceTemperature);
    }

    [Fact]
    public async Task Populatable_Life_TerraformWithPopulationFrom100()
    {
        // arrange
        var (context, p1) = await CreateEnvironmentAsync();

        var associatable = new Associatable() { PlayerId = p1 };
        var nameable = new Nameable() { Name = "XYZ" };
        var populatable = new Populatable(nameable, associatable) { Population = 1000, SurfaceTemperature = 100, Gravity = 1.0, MiningPercentage = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(99, populatable.SurfaceTemperature);
    }


    public async Task<(Context context, Guid p1)> CreateEnvironmentAsync()
    {
        Guid p1 = Guid.NewGuid();

        var turnEngine = await new TurnManagerBuilder(new ServiceCollection().BuildServiceProvider())
            .AddPlayer(new HumanPlayer(p1, "red", new Guid[0], 22, 1.0))
            .BuildAsync();

        return (turnEngine.CreateTurnContext(p1), p1);
    }
}
