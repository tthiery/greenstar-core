using System;

using GreenStar.Algorithms;
using GreenStar.Stellar;
using GreenStar.Traits;

using Microsoft.Extensions.Options;

namespace GreenStar.Cartography.Builder;

public class MilkyWayStellarGenerator : StellarGeneratorBase, IStellarGenerator
{
    public MilkyWayStellarGenerator(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
        : base(planetLifeOptions, nameGenerator)
    { }

    /// <summary>
    /// Generate a Milky Way like galaxy
    /// </summary>
    /// <remarks>
    /// Thanks to http://stackoverflow.com/questions/7323898/galaxy-generation-algorithm for the non-understandable algorithm ;)
    /// </remarks>
    [StellarGeneratorArgument("starsCount", "Number of Stars", 200)]
    [StellarGeneratorArgument("density", "Density of Stars", 100)]
    [StellarGeneratorArgument("armsCount", "Number of Arms", 4)]
    [StellarGeneratorArgument("rotation", "Rotation Speed", 0.1)]
    [StellarGeneratorArgument("spiralBow", "Spiral Bow", 15)]
    [StellarGeneratorArgument("armThickness", "Thickness of Arm", 75)]
    [StellarGeneratorArgument("lengthOfArm", "Length of Arm", 15)]
    public void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments)
    {
        int starsCount = ArgumentInt32(arguments, nameof(starsCount));
        int density = ArgumentInt32(arguments, nameof(density));
        int armsCount = ArgumentInt32(arguments, nameof(armsCount));
        double rotation = ArgumentDouble(arguments, nameof(rotation));
        int spiralBow = ArgumentInt32(arguments, nameof(spiralBow));
        int armThickness = ArgumentInt32(arguments, nameof(armThickness));
        int lengthOfArm = ArgumentInt32(arguments, nameof(lengthOfArm));

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

        double numberOfStarsEachArm = starsCount / armsCount;

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

                GenerateStellarObjectByMode(actorContext, mode, Coordinate.Zero, item =>
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
}
