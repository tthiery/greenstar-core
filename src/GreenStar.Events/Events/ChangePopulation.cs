using System;
using System.Globalization;
using System.Linq;

using GreenStar.Core;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine;
using GreenStar.Stellar;

namespace GreenStar.Events;

/// <summary>
/// Executor for the event ChangePopulation.
/// </summary>
public class ChangePopulation : EventTranscript
{
    /// <summary>
    /// Add a percental change of population to a random planet of the player.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="player"></param>
    /// <param name="argument">A decimal number (e.g. -0.40) for -40%</param>
    /// <param name="text"></param>
    public override void Execute(Context context, Player player, string text, string[] arguments)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (arguments.Length < 1)
        {
            throw new ArgumentException("too less arguments", nameof(arguments));
        }

        var planet = FindRandomPlanetOfPlayer(context, player.Id, true);

        double percentageChange;

        if (planet != null && double.TryParse(arguments[0], NumberStyles.Any, CultureInfo.InvariantCulture, out percentageChange))
        {
            long population = planet.Trait<Populatable>().Population;

            long populationChange = Convert.ToInt64(1.0f * population * percentageChange);

            planet.Trait<Populatable>().Population += populationChange;

            context.PlayerContext.SendMessageToPlayer(player.Id, context.TurnContext.Turn,
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
