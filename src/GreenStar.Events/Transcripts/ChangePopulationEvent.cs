using System;
using System.Globalization;
using System.Linq;

using GreenStar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.Stellar;
using System.Threading.Tasks;

namespace GreenStar.Transcripts;

/// <summary>
/// Executor for the event ChangePopulation.
/// </summary>
public class ChangePopulationEvent : EventTranscript
{
    private readonly string _text;
    private readonly string[] _arguments;

    public ChangePopulationEvent(string text, string[] arguments)
    {
        _text = text;
        _arguments = arguments;
    }
    /// <summary>
    /// Add a percental change of population to a random planet of the player.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="player"></param>
    /// <param name="argument">A decimal number (e.g. -0.40) for -40%</param>
    /// <param name="text"></param>
    public override Task ExecuteAsync(Context context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Player is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (_arguments.Length < 1)
        {
            throw new ArgumentException("too less arguments", nameof(_arguments));
        }

        var planet = FindRandomPlanetOfPlayer(context, context.Player.Id, true);

        double percentageChange;

        if (planet != null && double.TryParse(_arguments[0], NumberStyles.Any, CultureInfo.InvariantCulture, out percentageChange))
        {
            long population = planet.Trait<Populatable>().Population;

            long populationChange = Convert.ToInt64(1.0f * population * percentageChange);

            planet.Trait<Populatable>().Population += populationChange;

            context.PlayerContext.SendMessageToPlayer(context.Player.Id, context.TurnContext.Turn,
                text: string.Format(_text, planet.Trait<Associatable>().Name, percentageChange)
            );
        }

        return Task.CompletedTask;
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
