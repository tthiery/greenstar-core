using System;
using System.Linq;

using GreenStar.Core;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine;
using GreenStar.Stellar;

namespace GreenStar.Events;

/// <summary>
/// Executor for the event ChangePopulation.
/// </summary>
public class ChangePopulationEventExecutor : IEventExecutor
{
    /// <summary>
    /// Add a percental change of population to a random planet of the player.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="player"></param>
    /// <param name="argument">A decimal number (e.g. -0.40) for -40%</param>
    /// <param name="text"></param>
    public void Execute(Context context, Player player, string argument, string text)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentException("argument cannot be empty", nameof(argument));
        }

        var planet = FindRandomPlanetOfPlayer(context, player.Id, true);

        double percentageChange;

        if (planet != null && double.TryParse(argument, out percentageChange))
        {
            long population = planet.Trait<Populatable>().Population;

            long populationChange = Convert.ToInt64(1.0f * population * percentageChange);

            planet.Trait<Populatable>().Population += populationChange;

            context.PlayerContext.SendMessageToPlayer(
                playerId: player.Id,
                text: string.Format(text, planet.Trait<Associatable>().Name, percentageChange)
            );
        }
    }

    /// <summary>
    /// Search a random planet of the player
    /// </summary>
    /// <param name="context"></param>
    /// <param name="playerId"></param>
    /// <param name="requirePopulation"></param>
    /// <returns></returns>
    public static Planet? FindRandomPlanetOfPlayer(Context context, Guid playerId, bool requirePopulation)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var planet = context.ActorContext.AsQueryable()
            .OfType<Planet>()
            .With((Associatable a) => a.PlayerId == playerId)
            .With((Populatable p) => !requirePopulation || p.Population > 2)
            .TakeOneRandom();

        return planet;
    }
}
