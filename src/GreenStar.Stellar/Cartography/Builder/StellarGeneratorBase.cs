using System;
using System.Linq;

using GreenStar.Algorithms;
using GreenStar.Stellar;
using GreenStar.Traits;

using Microsoft.Extensions.Options;

namespace GreenStar.Cartography.Builder;

public abstract class StellarGeneratorBase
{
    private readonly IOptions<PlanetLifeOptions> _planetLifeOptions;
    protected readonly NameGenerator nameGenerator;

    public StellarGeneratorBase(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
    {
        _planetLifeOptions = planetLifeOptions;
        this.nameGenerator = nameGenerator;
    }

    protected int ArgumentInt32(StellarGeneratorArgument[] arguments, string name)
        => (int)(arguments.FirstOrDefault(arg => arg.Name == name)?.Value ?? throw new InvalidOperationException("invalid argument name"));
    protected double ArgumentDouble(StellarGeneratorArgument[] arguments, string name)
        => arguments.FirstOrDefault(arg => arg.Name == name)?.Value ?? throw new InvalidOperationException("invalid argument name");

    protected void SolarSystemInternal(IActorContext actorContext, Sun sun, int? planetCount)
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

            GeneratePlanet(actorContext, Coordinate.Zero, DiscoveryLevel.Unknown, planet =>
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

    protected void GenerateStellarObjectByMode(IActorContext actorContext, GeneratorMode mode, Coordinate coordinate, Action<Actor>? extensionBeforeAddition)
    {
        if (mode == GeneratorMode.Full)
        {
            // good add a star to the cluster
            var sun = new Sun();
            sun.Id = Guid.NewGuid();

            sun.Trait<Nameable>().Name = nameGenerator.GetNext("planet").Name; // TODO
            sun.Trait<Locatable>().SetPosition(coordinate); // set initial coordinate
            sun.Trait<Discoverable>().AddDiscoverer(Guid.Empty, DiscoveryLevel.LocationAware, 0); // add discovery
            if (extensionBeforeAddition is not null)
            {
                extensionBeforeAddition(sun);
            }
            actorContext.AddActor(sun);

            // add some planets
            SolarSystemInternal(actorContext, sun, null);
        }
        else if (mode == GeneratorMode.PlanetOnly)
        {
            GeneratePlanet(actorContext, coordinate, DiscoveryLevel.LocationAware, extensionBeforeAddition);
        }
    }

    protected Planet GeneratePlanet(IActorContext actorContext, Coordinate coordinate, DiscoveryLevel level, Action<Actor>? extensionBeforeAddition)
    {
        double gravity = PlanetAlgorithms.CreateRandomPlanetGravity(_planetLifeOptions.Value);
        double temperature = PlanetAlgorithms.CreateRandomePlanetTemperature(_planetLifeOptions.Value);
        var resources = PlanetAlgorithms.CreateInitialResourceAmountOfPlanet(_planetLifeOptions.Value, gravity, temperature);

        var planet = new Planet();
        planet.Id = Guid.NewGuid();

        planet.Trait<Nameable>().Name = nameGenerator.GetNext("planet").Name;
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