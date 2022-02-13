using System;

using GreenStar.Algorithms;

using Microsoft.Extensions.Options;

namespace GreenStar.Cartography.Builder;

public class GridStellarGenerator : StellarGeneratorBase, IStellarGenerator
{
    public GridStellarGenerator(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
        : base(planetLifeOptions, nameGenerator)
    { }

    /// <summary>
    /// Generates a big square of stars
    /// </summary>
    [StellarGeneratorArgument("starsCount", "Number of Stars", 200)]
    [StellarGeneratorArgument("density", "Density of Stars", 100)]
    public void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments)
    {
        int starsCount = ArgumentInt32(arguments, nameof(starsCount));
        int density = ArgumentInt32(arguments, nameof(density));

        long width = (long)Math.Floor((Math.Sqrt(starsCount) + 1) * density);

        var finalCount = Math.Floor(Math.Sqrt(starsCount));

        var rand = new Random();

        for (int idx = 0; idx < finalCount; idx++)
            for (int idy = 0; idy < finalCount; idy++)
            {
                var x = (long)(idx * density - 0.5 * density);
                var y = (long)(idy * density - 0.5 * density);

                GenerateStellarObjectByMode(actorContext, mode, new Coordinate(x, y), null);
            }
    }
}
