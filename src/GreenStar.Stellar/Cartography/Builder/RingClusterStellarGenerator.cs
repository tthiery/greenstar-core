using System;

using GreenStar.Algorithms;

using Microsoft.Extensions.Options;

namespace GreenStar.Cartography.Builder;

public class RingClusterStellarGenerator : StellarGeneratorBase, IStellarGenerator
{
    public RingClusterStellarGenerator(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
        : base(planetLifeOptions, nameGenerator)
    { }

    /// <summary>
    /// Generates a big donat style ring galaxy with small clusters
    /// </summary>
    [StellarGeneratorArgument("starsCount", "Number of Stars", 200)]
    [StellarGeneratorArgument("density", "Density of Stars", 100)]
    public void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments)
    {
        int starsCount = ArgumentInt32(arguments, nameof(starsCount));
        int density = ArgumentInt32(arguments, nameof(density));

        var numberOfSmallClusters = (int)Math.Ceiling(1.0 * starsCount / 10);

        starsCount = numberOfSmallClusters * 10;

        var smallClusterRadius = 20000;

        int circumstanceOfRing = numberOfSmallClusters * (smallClusterRadius * 2 + density / 20 * smallClusterRadius);

        var ringRadius = (int)(0.5 * circumstanceOfRing / Math.PI);

        var rand = new Random();

        int segment = (int)(360.0 / numberOfSmallClusters);

        for (int smallClusterIndex = 0; smallClusterIndex < numberOfSmallClusters; smallClusterIndex++)
        {
            int currentAngleToSmallCluster = segment * smallClusterIndex;

            int centerOfSmallClusterX = (int)(Math.Cos(currentAngleToSmallCluster * Math.PI / 180) * ringRadius + ringRadius);
            int centerOfSmallClusterY = (int)(Math.Sin(currentAngleToSmallCluster * Math.PI / 180) * ringRadius + ringRadius);

            for (int sunInSmallClusterIndex = 0; sunInSmallClusterIndex < 10; sunInSmallClusterIndex++)
            {
                int angle = rand.Next(0, 361) % 360;
                int distance = (int)(rand.NextDouble() * smallClusterRadius);

                int sunX = (int)(Math.Cos(angle * Math.PI / 180) * distance + centerOfSmallClusterX);
                int sunY = (int)(Math.Sin(angle * Math.PI / 180) * distance + centerOfSmallClusterY);

                GenerateStellarObjectByMode(actorContext, mode, new Coordinate(sunX, sunY), null);
            }
        }
    }
}
