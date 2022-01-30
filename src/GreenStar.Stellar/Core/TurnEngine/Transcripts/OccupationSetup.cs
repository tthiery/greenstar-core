using System;

using GreenStar.Algorithms;
using GreenStar.Core.Traits;
using GreenStar.Stellar;

namespace GreenStar.Core.TurnEngine.Transcripts;

public class OccupationSetup : SetupTranscript
{
    public override void Execute(Context context)
    {
        var rand = new Random();

        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            Planet? homePlanet = null;

            while (homePlanet is null)
            {
                var planet = context.ActorContext.GetRandomActor(actor => actor is Planet) as Planet;

                if (planet is not null)
                {
                    var associatable = planet.Trait<Associatable>();

                    if (associatable.PlayerId == Guid.Empty)
                    {
                        associatable.PlayerId = player.Id;
                        planet.Trait<Populatable>().Population = PlanetAlgorithms.MaxPopulation;
                        planet.Trait<Populatable>().Gravity = player.IdealGravity;
                        planet.Trait<Populatable>().SurfaceTemperature = player.IdealTemperature;

                        planet.Trait<Discoverable>().AddDiscoverer(player.Id, DiscoveryLevel.PropertyAware, 0);
                        homePlanet = planet;

                        var orbiting = planet.Trait<Orbiting>();

                        if (orbiting.Host != Guid.Empty)
                        {
                            var sun = context.ActorContext.GetActor(orbiting.Host) as Sun;

                            if (sun != null)
                            {
                                sun.Trait<Discoverable>().AddDiscoverer(player.Id, DiscoveryLevel.PropertyAware, 0);

                                var planets = context.ActorContext.GetActors<Planet, Orbiting>(orbiting => orbiting.Host == sun.Id);

                                foreach (var otherPlanet in planets)
                                {
                                    otherPlanet.Trait<Discoverable>().AddDiscoverer(player.Id, DiscoveryLevel.PropertyAware, 0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}