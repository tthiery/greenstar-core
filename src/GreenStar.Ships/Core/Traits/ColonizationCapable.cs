using System;
using GreenStar.Stellar;

namespace GreenStar.Core.Traits
{
    public class ColonizationCapable : Trait
    {
        private readonly Locatable _locatable;
        private readonly Associatable _associatable;

        public bool IsLoaded { get; set; } = true;

        public ColonizationCapable(Locatable locatable, Associatable associatable)
        {
            this._locatable = locatable ?? throw new System.ArgumentNullException(nameof(locatable));
            this._associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
        }

        public void Colonize(IActorContext actorContext, IPlayerContext playerContext)
        {
            if (actorContext == null)
            {
                throw new System.ArgumentNullException(nameof(actorContext));
            }

            if (!_locatable.HasOwnPosition)
            {
                if (actorContext.GetActor(_locatable.HostLocationActorId) is Planet planet)
                {
                    if (IsLoaded)
                    {
                        ColonizePlanet(planet, _associatable.PlayerId);

                        playerContext.SendMessageToPlayer(_associatable.PlayerId, text: $"You colonized {planet.Trait<Associatable>().Name}.");
                    }
                    else
                    {
                        IsLoaded = RecruitColonists(planet, _associatable.PlayerId);
                    }

                }
            }
        }

        private static void ColonizePlanet(Planet planet, Guid playerId)
        {
            if (planet == null)
            {
                throw new ArgumentNullException(nameof(planet));
            }

            var association = planet.Trait<Associatable>() ?? throw new Exception("Invalid State");

            if (!association.IsOwnedByAnyPlayer())
            {
                var population = planet.Trait<Populatable>() ?? throw new Exception("Invalid State");

                population.Population = 10;
                association.PlayerId = playerId;
            }
        }

        private bool RecruitColonists(Planet planet, Guid playerId)
        {
            if (planet == null)
            {
                throw new ArgumentNullException(nameof(planet));
            }

            bool result = false;

            var association = planet.Trait<Associatable>() ?? throw new Exception("Invalid State");

            if (association.IsOwnedByPlayer(playerId))
            {
                var population = planet.Trait<Populatable>() ?? throw new Exception("Invalid State");

                result = (population.Population >= 10);
            }

            return result;
        }
    }
}