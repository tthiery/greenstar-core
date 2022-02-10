using System;
using Microsoft.Extensions.Options;
using GreenStar.Algorithms;
using System.Collections.Generic;

namespace GreenStar.Cartography.Builder;

public class RoundClusterStellarGenerator : StellarGeneratorBase, IStellarGenerator
{
    public RoundClusterStellarGenerator(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
        : base(planetLifeOptions, nameGenerator)
    { }

    /// <summary>
    /// Generates a big round cluster of stars in the starfield
    /// </summary>
    [StellarGeneratorArgument("starsCount", "Number of Stars", 200)]
    [StellarGeneratorArgument("density", "Density of Stars", 100)]
    public void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments)
    {
        int starsCount = ArgumentInt32(arguments, nameof(starsCount));
        int density = ArgumentInt32(arguments, nameof(density));

        // assume a grid (square) between stars => we have a area
        // .. get the side of a square
        double width = Math.Floor(Math.Sqrt(starsCount));
        // .. inject the density factor to the width and calculate the area of the square
        double area = Math.Pow(width * density, 2);
        // now build a appropriate circle radius
        double intermediateRadius = Math.Sqrt(area / Math.PI);
        // expand the area a bit
        int radius = (int)(intermediateRadius * 1.3) * 100;

        var rand = new Random();

        for (int idx = 0; idx < starsCount; idx++)
        {
            int x = rand.Next(0, 2 * radius) - radius;
            int y = rand.Next(0, 2 * radius) - radius;

            double deltaFromCenter = Math.Sqrt(x * x + y * y);

            if (deltaFromCenter <= radius)
            {
                // good add a star to the cluster
                GenerateStellarObjectByMode(actorContext, mode, new Coordinate(x + radius, y + radius), null);
            }
            else
            {
                // bad
                idx--;
            }
        }
    }
}
