using System;
using GreenStar.Algorithms;

namespace GreenStar.Core.Traits
{
    public class Populatable : Trait
    {
        private readonly Associatable _associatable;

        /// <summary>
        /// Population living in/on the actor
        /// </summary>
        public long Population { get; set; }

        /// <summary>
        /// The gravity affecting the population.
        /// </summary>
        public double Gravity { get; set; }

        /// <summary>
        /// The temperature affecting the population.
        /// </summary>
        public double SurfaceTemperature { get; set; }

        /// <summary>
        /// The percentage of energy on that planet in mining
        /// </summary>
        public int MiningPercentage { get; set; }

        public Populatable(Associatable associatable)
        {
            _associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
        }

        public void Life(IPlayerContext playerContext)
        {
            if (_associatable.IsOwnedByAnyPlayer())
            {
                Terraform(playerContext);

                GrowPopulation(playerContext);
            }
        }

        /// <summary>
        /// Adjust the temperature as a respose on enabled terraforming
        /// </summary>
        /// <param name="planet"></param>
        private void Terraform(IPlayerContext playerContext)
        {
            if (Population > 0)
            {
                var (_, idealTemperature) = RetrieveIdealConditionForPlayer(playerContext, _associatable.PlayerId);

                if (SurfaceTemperature != idealTemperature)
                {
                    double newTemperature = PlanetAlgorithms.CalculateNewTemperature(SurfaceTemperature, idealTemperature);

                    SurfaceTemperature = newTemperature;

                    if (SurfaceTemperature == idealTemperature)
                    {
                        playerContext.SendMessageToPlayer(
                            playerId: _associatable.PlayerId,
                            type: "Info",
                            text: $"You have finished the terraforming of {_associatable.Name}"
                        );
                    }
                }
            }
        }

        private void GrowPopulation(IPlayerContext playerContext)
        {
            long population = Population;

            if (population > 0)
            {
                var (idealGravity, idealTemperature) = RetrieveIdealConditionForPlayer(playerContext, _associatable.PlayerId);

                var newPopulation = PlanetAlgorithms.CalculateNewPopulation(
                    population,
                    Gravity, SurfaceTemperature,
                    idealGravity, idealTemperature,
                    100);

                Population = newPopulation;
            }
        }

        private (double idealGravity, double idealTemperature) RetrieveIdealConditionForPlayer(IPlayerContext playerContext, Guid playerId)
        {
            if (playerContext == null)
            {
                throw new ArgumentNullException(nameof(playerContext));
            }

            var player = playerContext.GetPlayer(playerId);

            return (player?.IdealGravity ?? 1, player?.IdealTemperature ?? 20);
        }
    }
}