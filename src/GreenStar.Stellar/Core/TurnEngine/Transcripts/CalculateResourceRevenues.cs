using System;
using System.Collections.Generic;
using System.Linq;
using GreenStar.Core.Resources;
using GreenStar.Algorithms;
using GreenStar.Stellar;
using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    /// <summary>
    /// Iterate each revueable business (e.g. planet, manufacturing site, partnerships, etc.) and collect resources
    /// </summary>
    public class CalculateResourceRevenues : TurnTranscript
    {
        /// <summary>
        /// Calculate Mining of resources
        /// </summary>
        public override void Execute(Context context)
        {
            if (this.Game == null)
            {
                throw new InvalidOperationException("CalculateResourceRevenues.Game is not set");
            }
            if (this.IntermediateData == null)
            {
                throw new InvalidOperationException("CalculateResourceRevenues.IntermediateData is not set");
            }

            var invoices = this.IntermediateData["Billing"] as Dictionary<Guid, Invoice>;

            if (invoices == null)
            {
                throw new InvalidOperationException("Billing element not set");
            }

            var planets = Game.Actors
                .OfType<Planet>()
                .Where(x => x.Trait<Associatable>()?.PlayerId != Guid.Empty);

            foreach (var planet in planets)
            {
                var associatable = planet.Trait<Associatable>() ?? throw new Exception("Invalid State");

                var overallRevenueOfPlanet = new ResourceAmount();
                
                var resourceGathered = MineResources(planet);
                if (resourceGathered != null)
                {
                    overallRevenueOfPlanet += resourceGathered;

                    CheckIfAllResourcesStrippedAndAdjustDevelopmentRatio(planet, resourceGathered, associatable);
                }

                var finanicalIncome = FinancialIncome(planet);
                if (finanicalIncome != null)
                {
                    overallRevenueOfPlanet += finanicalIncome;
                }

                overallRevenueOfPlanet.Name = $"Revenue on {associatable.Name}";

                Invoice invoiceOfPlayer = invoices[associatable.PlayerId];

                if (invoiceOfPlayer == null || invoiceOfPlayer.Items == null)
                {
                    throw new InvalidOperationException("Invoice of player not present.");
                }

                invoiceOfPlayer.Items.Add(overallRevenueOfPlanet);
            }
        }

        /// <summary>
        /// Set the development ratio to 0% mining when the resources are empty.
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="resourceGathered"></param>
        private void CheckIfAllResourcesStrippedAndAdjustDevelopmentRatio(Planet planet, ResourceAmount resourceGathered, Associatable associatable)
        {
            if (planet == null)
            {
                throw new ArgumentNullException("planet");
            }
            if (this.Game == null)
            {
                throw new InvalidOperationException("CalculateResourceRevenues.Game is not set");
            }
            if (resourceGathered == null)
            {
                throw new ArgumentNullException("resourceGathered");
            }

            if (resourceGathered.Values.Any(x => x.Value > 0) && planet.Trait<Resourceful>().Resources.Values.All(x => x.Value == 0))
            {
                PlayerContext.SendMessageToPlayer(associatable.PlayerId,
                    text: $"You stripped all resources from {associatable.Name}"
                );

                planet.Trait<Populatable>().MiningPercentage = 0;
            }
        }

        /// <summary>
        /// Creates a financial income.
        /// </summary>
        /// <param name="planet"></param>
        /// <returns></returns>
        private static ResourceAmount FinancialIncome(Planet planet)
        {
            if (planet == null)
            {
                throw new ArgumentNullException("planet");
            }

            long population = planet.Trait<Populatable>().Population;

            int revenue = PlanetAlgorithms.CalculateRevenueOfPlanet(100, population);

            ResourceAmount amount = new ResourceAmount();

            amount[ResourceConstants.Money] = revenue;

            return amount;
        }

        /// <summary>
        /// Subtract a certain amount of resources from the planets resouces
        /// </summary>
        /// <param name="planet"></param>
        /// <returns></returns>
        private static ResourceAmount MineResources(Planet planet)
        {
            if (planet == null)
            {
                throw new ArgumentNullException("planet");
            }

            ResourceAmount result = null;

            int investment = planet.Trait<Populatable>().MiningPercentage;

            if (investment > 0)
            {
                var resourcesOfPlanet = planet.Trait<Resourceful>().Resources;

                double percentageOfResourcesToBeTaken = PlanetAlgorithms.CalculateMiningPercentage(investment);

                result = resourcesOfPlanet * percentageOfResourcesToBeTaken;

                foreach (var resource in result.Values)
                {
                    if (resource.Value == 0 && resourcesOfPlanet[resource.Resource] > 0) // rounding issue
                    {
                        resource.Value = 1;
                    }
                }

                planet.Trait<Resourceful>().Resources -= result;

                if (result.Values.All(x => x.Value == 0))
                {
                    result = null;
                }
            }

            return result;
        }
    }
}
