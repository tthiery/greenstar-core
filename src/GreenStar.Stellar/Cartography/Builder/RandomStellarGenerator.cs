using System;
using Microsoft.Extensions.Options;
using GreenStar.Algorithms;
using System.Collections.Generic;

namespace GreenStar.Cartography.Builder;

public class RandomStellarGenerator : StellarGeneratorBase, IStellarGenerator
{
    public RandomStellarGenerator(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
        : base(planetLifeOptions, nameGenerator)
    { }

    /// <summary>
    /// Generates a random star field
    /// </summary>
    [StellarGeneratorArgument("starsCount", "Number of Stars", 200)]
    [StellarGeneratorArgument("density", "Density of Stars", 100)]
    public void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments)
    {
        int starsCount = ArgumentInt32(arguments, nameof(starsCount));
        int density = ArgumentInt32(arguments, nameof(density));

        long width = (long)Math.Floor(Math.Sqrt(starsCount) * density);

        var rand = new Random();

        for (int idx = 0; idx < starsCount; idx++)
        {
            var x = rand.NextInt64(0, width);
            var y = rand.NextInt64(0, width);

            GenerateStellarObjectByMode(actorContext, mode, new Coordinate(x, y), null);
        }
    }
}
