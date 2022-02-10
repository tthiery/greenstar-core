using System;

using GreenStar.Resources;

namespace GreenStar.Algorithms;

public class PlanetLifeOptions
{
    /// <summary>
    /// Minimum temperature on a planet
    /// </summary>
    public double MinTemperature = -80;
    /// <summary>
    /// Maximum temperature on a planet
    /// </summary>
    public double MaxTemperature = 120;
    /// <summary>
    /// Minimum gravity on a planet
    /// </summary>
    public double MinGravity = 0.20;
    /// <summary>
    /// Maximum gravity on a planet
    /// </summary>
    public double MaxGravity = 5.00;
    /// <summary>
    /// Minimum metal on a planet
    /// </summary>
    public int MinMetalPlanet = 500;
    /// <summary>
    /// Maximum metal on a planet
    /// </summary>
    public int MaxMetalPlanet = 50000;
    /// <summary>
    /// Maximum population on a planet
    /// </summary>
    public double MaxPopulation = 5_000_000_000;
    /// <summary>
    /// How many people return one money unit
    /// </summary>
    /// <value></value>
    public double PersonPerMoney { get; set; } = 20;
    /// <summary>
    /// How much a colony cost to maintain
    /// </summary>
    /// <value></value>
    public int ColonyCost { get; set; } = 100;
}

/// <summary>
/// Algorithms for planets
/// </summary>
public static class PlanetAlgorithms
{
    #region Private Fields
    /// <summary>
    /// Random number used in all random algorithms
    /// </summary>
    private static readonly Random random = new Random();
    #endregion

    #region Algorithms
    /// <summary>
    /// Creates a random gravity
    /// </summary>
    /// <returns></returns>
    public static double CreateRandomPlanetGravity(PlanetLifeOptions planetLifeOptions)
    {
        double result = planetLifeOptions.MinGravity + random.NextDouble() * (planetLifeOptions.MaxGravity - planetLifeOptions.MinGravity);

        return result;
    }

    /// <summary>
    /// Creates a random temperature
    /// </summary>
    /// <returns></returns>
    public static double CreateRandomePlanetTemperature(PlanetLifeOptions planetLifeOptions)
    {
        double result = random.NextDouble() * planetLifeOptions.MaxTemperature - planetLifeOptions.MinTemperature;

        return result;
    }

    /// <summary>
    /// Creates a random metal amount depending on the gravity of the planet.
    /// </summary>
    /// <param name="gravity"></param>
    /// <param name="temperature"></param>
    /// <returns></returns>
    public static ResourceAmount CreateInitialResourceAmountOfPlanet(PlanetLifeOptions planetLifeOptions, double gravity, double temperature)
    {
        var random = new Random();

        int max = Convert.ToInt32(planetLifeOptions.MaxMetalPlanet * (gravity / planetLifeOptions.MaxGravity));

        int metal = random.Next(planetLifeOptions.MinMetalPlanet, max);

        var amount = new ResourceAmount("Planet Resources", new[] { new ResourceAmountItem(ResourceConstants.Metal, metal) });

        return amount;
    }

    /// <summary>
    /// Calculates the new population after a turn of growth.
    /// </summary>
    /// <param name="currentPopulation"></param>
    /// <param name="currentGravity"></param>
    /// <param name="currentTemperature"></param>
    /// <param name="idealGravity"></param>
    /// <param name="idealTemperature"></param>
    /// <param name="livingPercentage"></param>
    /// <returns></returns>
    public static long CalculateNewPopulation(PlanetLifeOptions planetLifeOptions, long currentPopulation, double currentGravity, double currentTemperature, double idealGravity, double idealTemperature, int livingPercentage)
    {
        double populationGrowthRate = 0.50;

        double punishmentForHighGravity = Math.Pow(3, Math.Abs(currentGravity - idealGravity) / 0.1) * 5000;

        double punishmentForBadTemperatur = Math.Pow(7, Math.Abs(currentTemperature - idealTemperature) / 20) * 5000;

        double maxPopulation = (planetLifeOptions.MaxPopulation - punishmentForHighGravity) - punishmentForBadTemperatur;

        if (maxPopulation < 150)
        {
            maxPopulation = 150;
        }

        double newPopulation = 1.0 * currentPopulation * (1.0 + (populationGrowthRate * livingPercentage * 0.01));

        long result = currentPopulation;

        if (newPopulation < maxPopulation && newPopulation > currentPopulation)
        {
            result = Convert.ToInt64(newPopulation);
        }
        else if (newPopulation > currentPopulation)
        {
            result = Convert.ToInt64(maxPopulation);
        }
        return result;
    }

    /// <summary>
    /// Calculates the new temperature of a planet after a turn of terraforming
    /// </summary>
    /// <param name="currentTemperature"></param>
    /// <param name="idealTemperature"></param>
    /// <returns></returns>
    public static double CalculateNewTemperature(double currentTemperature, double idealTemperature)
    {
        double factor = 1.0;

        double direction = 1.0;

        if (idealTemperature < currentTemperature)
        {
            direction = -1.0;
        }

        double appliedValue = factor * direction * 1.0;

        double newValue = currentTemperature + appliedValue;

        if (Math.Abs(idealTemperature - newValue) < 1.0)
        {
            newValue = idealTemperature;
        }

        return newValue;
    }

    /// <summary>
    /// Calculates the revenue of a planet
    /// </summary>
    /// <param name="livingPercentage"></param>
    /// <param name="population"></param>
    /// <returns></returns>
    public static int CalculateRevenueOfPlanet(PlanetLifeOptions config, int livingPercentage, long population)
    {
        int revenue = Convert.ToInt32((0.01 * livingPercentage * population) / config.PersonPerMoney) - config.ColonyCost;

        return revenue;
    }

    /// <summary>
    /// Calculate the current mining percentage
    /// </summary>
    /// <param name="miningPercentage"></param>
    /// <returns></returns>
    public static double CalculateMiningPercentage(int miningPercentage)
    {
        double percentageOfResourcesToBeTaken = 0.1 * 0.01 * miningPercentage;

        return percentageOfResourcesToBeTaken;
    }
    #endregion
}
