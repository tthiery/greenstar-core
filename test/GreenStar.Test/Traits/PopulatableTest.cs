using System;

using GreenStar.Algorithms;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Traits;

public class PopulatableTest
{
    [Fact]
    public void Populatable_Life_PopulationGrowNotWithoutPopulation()
    {
        // arrange
        var (context, p1) = CreateEnvironment();

        var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
        var populatable = new Populatable(associatable) { Population = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(0, populatable.Population);
    }

    [Fact]
    public void Populatable_Life_PopulationGrowWithoutIdealConditionOn1000()
    {
        // arrange
        var (context, p1) = CreateEnvironment();

        var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
        var populatable = new Populatable(associatable) { Population = 1000, SurfaceTemperature = 22, Gravity = 1.0, MiningPercentage = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(1500, populatable.Population);
    }

    [Fact]
    public void Populatable_Life_PopulationGrowWithUpperLimit()
    {
        // arrange
        var (context, p1) = CreateEnvironment();

        var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
        var populatable = new Populatable(associatable) { Population = 4_500_000_000, SurfaceTemperature = 22, Gravity = 1.0, MiningPercentage = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(4_999_990_000, populatable.Population);
    }

    [Fact]
    public void Populatable_Life_TerraformNotWithoutPopulation()
    {
        // arrange
        var (context, p1) = CreateEnvironment();

        var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
        var populatable = new Populatable(associatable) { Population = 0, SurfaceTemperature = 100, Gravity = 1.0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(100, populatable.SurfaceTemperature);
    }

    [Fact]
    public void Populatable_Life_TerraformWithPopulationFrom100()
    {
        // arrange
        var (context, p1) = CreateEnvironment();

        var associatable = new Associatable() { PlayerId = p1, Name = "XYZ" };
        var populatable = new Populatable(associatable) { Population = 1000, SurfaceTemperature = 100, Gravity = 1.0, MiningPercentage = 0 };

        // act
        populatable.Life(context, new PlanetLifeOptions());

        // assert
        Assert.Equal(99, populatable.SurfaceTemperature);
    }


    public (Context context, Guid p1) CreateEnvironment()
    {
        Guid p1 = Guid.NewGuid();

        var turnEngine = new TurnManagerBuilder(new ServiceCollection().BuildServiceProvider())
            .AddPlayer(new HumanPlayer(p1, "red", new Guid[0], 22, 1.0))
            .Build();

        return (turnEngine.CreateTurnContext(p1), p1);
    }
}
