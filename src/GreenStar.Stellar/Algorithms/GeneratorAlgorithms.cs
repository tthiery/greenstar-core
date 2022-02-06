using System;

using GreenStar;
using GreenStar.Cartography;
using GreenStar.Traits;
using GreenStar.Stellar;

namespace GreenStar.Algorithms;

public static class GeneratorAlgorithms
{
    /// <summary>
    /// Generate a Milky Way like galaxy
    /// </summary>
    /// <remarks>
    /// Thanks to http://stackoverflow.com/questions/7323898/galaxy-generation-algorithm for the non-understandable algorithm ;)
    /// </remarks>
    public static void MilkyWay(IActorContext actorContext, GeneratorMode mode, NameGenerator nameGenerator, int starCount, int density, int armsCount, double rotation, int spiralBow, int armThickness, int lengthOfArm)
    {
        if (rotation < 0 || rotation > 1)
        {
            throw new InvalidOperationException("rotation needs to be between 0.0 and 1.0");
        }
        if (armThickness < 1 || armThickness > 100)
        {
            throw new InvalidOperationException("armThickness needs to be between 0 and 100");
        }

        double radius = 1000 * density;

        var center = new ExactLocation();
        center.Id = Guid.NewGuid();
        center.Trait<Locatable>().SetPosition(new Coordinate((long)radius, (long)radius));
        actorContext.AddActor(center);


        double radiusMultiplier = radius / lengthOfArm;

        var r = new Random();

        double angleOfEachArm = (double)(360 / armsCount);

        double thicknessPercent = angleOfEachArm * 0.5 * armThickness / 100;

        double numberOfStarsEachArm = starCount / armsCount;

        for (int armIndex = 0; armIndex < armsCount; armIndex++)
        {
            double angleOfCurrentArm = angleOfEachArm * armIndex;

            for (int sunIndex = 0; sunIndex < numberOfStarsEachArm; sunIndex++)
            {
                // a point in the arm (avoiding the first 15%)
                double positionInArm = r.NextDouble() * lengthOfArm * 0.85 + lengthOfArm * 0.15;

                // the derivation from the line
                double spreadFromArm = r.NextDouble() * thicknessPercent;

                // the bow in the long line
                double positionBasedDerivation = positionInArm * spiralBow;

                double totalDegree = angleOfCurrentArm + spreadFromArm - positionBasedDerivation;

                // double armX = radiusMultiplier * positionInArm * Math.Cos(totalDegree * Math.PI / 180);
                // double armY = radiusMultiplier * positionInArm * Math.Sin(totalDegree * Math.PI / 180);

                // var coordinate = new Coordinate((int)(armX + radius), (int)(armY + radius));

                GenerateStellarObjectByMode(actorContext, mode, nameGenerator, Coordinate.Zero, item =>
                {
                    if (!item.TryGetTrait<Orbiting>(out var orbit))
                    {
                        orbit = item.AddTrait<Orbiting>(); // suns typically do not have one
                    }

                    orbit.Host = center.Id;
                    orbit.CurrentDegree = totalDegree;
                    orbit.SpeedDegree = rotation;
                    orbit.Distance = radiusMultiplier * positionInArm;

                    orbit.Move(actorContext, center);
                });
            }
        }
    }

    /// <summary>
    /// Generates a big donat style ring galaxy with small clusters
    /// </summary>
    public static void RingCluster(IActorContext actorContext, GeneratorMode mode, NameGenerator nameGenerator, int starCount, int density)
    {
        var numberOfSmallClusters = (int)Math.Ceiling(1.0 * starCount / 10);

        starCount = numberOfSmallClusters * 10;

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

                GenerateStellarObjectByMode(actorContext, mode, nameGenerator, new Coordinate(sunX, sunY), null);
            }
        }
    }

    /// <summary>
    /// Generates a big round cluster of stars in the starfield
    /// </summary>
    public static void RoundCluster(IActorContext actorContext, GeneratorMode mode, NameGenerator nameGenerator, int starsCount, int density)
    {
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
                GenerateStellarObjectByMode(actorContext, mode, nameGenerator, new Coordinate(x + radius, y + radius), null);
            }
            else
            {
                // bad
                idx--;
            }
        }
    }

    public static void SolarSystem(IActorContext actorContext, GeneratorMode mode, NameGenerator nameGenerator, int planetCount)
    {
        var sun = new Sun();
        sun.Id = Guid.NewGuid();

        sun.Trait<Nameable>().Name = nameGenerator.GetNext("planets").Name; // TODO
        sun.Trait<Locatable>().SetPosition(new Coordinate(1000, 1000));
        sun.Trait<Discoverable>().AddDiscoverer(Guid.Empty, DiscoveryLevel.LocationAware, 0); // add discovery
        actorContext.AddActor(sun);

        SolarSystemInternal(actorContext, sun, nameGenerator, planetCount);
    }
    private static void SolarSystemInternal(IActorContext actorContext, Sun sun, NameGenerator nameGenerator, int? planetCount)
    {
        int MaxNumber = 10;

        var rand = new Random();

        int actualNumberOfPlanets = (planetCount is not null) ? planetCount.Value : rand.Next(0, MaxNumber);

        int orbitDistance = 50;

        for (int idx = 0; idx < actualNumberOfPlanets; idx++)
        {
            orbitDistance += rand.Next(50, 100);
            short degree = (short)rand.Next(-360, 360);
            int degreePerRound = rand.Next(1, 12);

            GeneratePlanet(actorContext, nameGenerator, Coordinate.Zero, DiscoveryLevel.Unknown, planet =>
             {
                 var orbit = planet.Trait<Orbiting>();

                 orbit.Host = sun.Id;
                 orbit.CurrentDegree = degree;
                 orbit.SpeedDegree = degreePerRound;
                 orbit.Distance = orbitDistance;

                 orbit.Move(actorContext, sun); // ensure initial relative coordinate
             });

        }
    }

    private static void GenerateStellarObjectByMode(IActorContext actorContext, GeneratorMode mode, NameGenerator nameGenerator, Coordinate coordinate, Action<Actor>? extensionBeforeAddition)
    {
        if (mode == GeneratorMode.Full)
        {
            // good add a star to the cluster
            var sun = new Sun();
            sun.Id = Guid.NewGuid();

            sun.Trait<Nameable>().Name = nameGenerator.GetNext("planets").Name; // TODO
            sun.Trait<Locatable>().SetPosition(coordinate); // set initial coordinate
            sun.Trait<Discoverable>().AddDiscoverer(Guid.Empty, DiscoveryLevel.LocationAware, 0); // add discovery
            if (extensionBeforeAddition is not null)
            {
                extensionBeforeAddition(sun);
            }
            actorContext.AddActor(sun);

            // add some planets
            SolarSystemInternal(actorContext, sun, nameGenerator, null);
        }
        else if (mode == GeneratorMode.PlanetOnly)
        {
            GeneratePlanet(actorContext, nameGenerator, coordinate, DiscoveryLevel.LocationAware, extensionBeforeAddition);
        }
    }

    private static Planet GeneratePlanet(IActorContext actorContext, NameGenerator nameGenerator, Coordinate coordinate, DiscoveryLevel level, Action<Actor>? extensionBeforeAddition)
    {
        double gravity = PlanetAlgorithms.CreateRandomPlanetGravity();
        double temperature = PlanetAlgorithms.CreateRandomePlanetTemperature();
        var resources = PlanetAlgorithms.CreateInitialResourceAmountOfPlanet(gravity, temperature);

        var planet = new Planet();
        planet.Id = Guid.NewGuid();

        planet.Trait<Nameable>().Name = nameGenerator.GetNext("planets").Name;
        planet.Trait<Populatable>().Gravity = gravity;
        planet.Trait<Populatable>().SurfaceTemperature = temperature;
        planet.Trait<Resourceful>().Resources = resources;
        planet.Trait<Locatable>().SetPosition(coordinate);
        planet.Trait<Discoverable>().AddDiscoverer(Guid.Empty, level, 0); // add discovery

        if (extensionBeforeAddition is not null)
        {
            extensionBeforeAddition(planet);
        }

        actorContext.AddActor(planet);

        return planet;
    }
}