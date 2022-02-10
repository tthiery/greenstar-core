using System;

using GreenStar.Traits;
using GreenStar.Stellar;
using Microsoft.Extensions.Options;
using GreenStar.Algorithms;
using System.Collections.Generic;

namespace GreenStar.Cartography.Builder;

public class SolarSystemStellarGenerator : StellarGeneratorBase, IStellarGenerator
{
    public SolarSystemStellarGenerator(IOptions<PlanetLifeOptions> planetLifeOptions, NameGenerator nameGenerator)
        : base(planetLifeOptions, nameGenerator)
    { }

    [StellarGeneratorArgument("planetCount", "Number of Stars", 9)]
    public void Generate(IActorContext actorContext, GeneratorMode mode, StellarGeneratorArgument[] arguments)
    {
        int planetCount = ArgumentInt32(arguments, nameof(planetCount));

        var sun = new Sun();
        sun.Id = Guid.NewGuid();

        sun.Trait<Nameable>().Name = nameGenerator.GetNext("planet").Name; // TODO
        sun.Trait<Locatable>().SetPosition(new Coordinate(1000, 1000));
        sun.Trait<Discoverable>().AddDiscoverer(Guid.Empty, DiscoveryLevel.LocationAware, 0); // add discovery
        actorContext.AddActor(sun);

        SolarSystemInternal(actorContext, sun, planetCount);
    }

}
