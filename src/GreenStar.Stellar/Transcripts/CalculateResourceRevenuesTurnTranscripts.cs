using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Algorithms;
using GreenStar.Resources;
using GreenStar.Traits;
using GreenStar.Stellar;
using GreenStar.TurnEngine;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GreenStar.Transcripts;

/// <summary>
/// Iterate each revueable business (e.g. planet, manufacturing site, partnerships, etc.) and collect resources
/// </summary>
public class CalculateResourceRevenuesTurnTranscripts : TurnTranscript
{
    private readonly IOptions<PlanetLifeOptions> _planetLifeOptions;

    public CalculateResourceRevenuesTurnTranscripts(IOptions<PlanetLifeOptions> planetLifeOptions)
    {
        _planetLifeOptions = planetLifeOptions;
    }
    /// <summary>
    /// Calculate Mining of resources
    /// </summary>
    public override Task ExecuteAsync(Context context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (this.IntermediateData == null)
        {
            throw new InvalidOperationException("CalculateResourceRevenues.IntermediateData is not set");
        }

        if (this.IntermediateData.ContainsKey("Billing"))
        {

            var invoices = this.IntermediateData["Billing"] as Dictionary<Guid, Invoice>;

            if (invoices == null)
            {
                throw new InvalidOperationException("Billing element not set");
            }

            var planets = context.ActorContext.AsQueryable()
                .OfType<Planet>()
                .With<Planet, Associatable>(associatable => associatable?.PlayerId != Guid.Empty);

            foreach (var planet in planets)
            {
                var associatable = planet.Trait<Associatable>() ?? throw new Exception("Invalid State");

                var overallRevenueOfPlanet = new ResourceAmount($"Revenue on {associatable.Name}", Array.Empty<ResourceAmountItem>());

                var resourceGathered = MineResources(planet);
                if (resourceGathered != null)
                {
                    overallRevenueOfPlanet += resourceGathered;

                    CheckIfAllResourcesStrippedAndAdjustDevelopmentRatio(context, planet, resourceGathered, associatable);
                }

                var finanicalIncome = FinancialIncome(planet);
                if (finanicalIncome != null)
                {
                    overallRevenueOfPlanet += finanicalIncome;
                }

                Invoice invoiceOfPlayer = invoices[associatable.PlayerId];

                if (invoiceOfPlayer == null || invoiceOfPlayer.Items == null)
                {
                    throw new InvalidOperationException("Invoice of player not present.");
                }

                invoiceOfPlayer.Items.Add(overallRevenueOfPlanet);
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Set the development ratio to 0% mining when the resources are empty.
    /// </summary>
    /// <param name="planet"></param>
    /// <param name="resourceGathered"></param>
    private void CheckIfAllResourcesStrippedAndAdjustDevelopmentRatio(Context context, Planet planet, ResourceAmount resourceGathered, Associatable associatable)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (planet == null)
        {
            throw new ArgumentNullException("planet");
        }
        if (resourceGathered == null)
        {
            throw new ArgumentNullException("resourceGathered");
        }

        if (resourceGathered.Values.Any(x => x.Value > 0) && planet.Trait<Resourceful>().Resources.Values.All(x => x.Value == 0))
        {
            context.PlayerContext.SendMessageToPlayer(associatable.PlayerId, context.TurnContext.Turn,
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
    private ResourceAmount FinancialIncome(Planet planet)
    {
        if (planet == null)
        {
            throw new ArgumentNullException("planet");
        }

        long population = planet.Trait<Populatable>().Population;

        int revenue = PlanetAlgorithms.CalculateRevenueOfPlanet(_planetLifeOptions.Value, 100, population);

        var amount = new ResourceAmount("Planet Income", new[] { new ResourceAmountItem(ResourceConstants.Money, revenue) });

        return amount;
    }

    /// <summary>
    /// Subtract a certain amount of resources from the planets resouces
    /// </summary>
    /// <param name="planet"></param>
    /// <returns></returns>
    private static ResourceAmount? MineResources(Planet planet)
    {
        if (planet == null)
        {
            throw new ArgumentNullException("planet");
        }

        ResourceAmount? result = null;

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
                    result = result.WithResource(resource.Resource, 1);
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
