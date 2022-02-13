using System;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

/// <summary>
/// Event Executor for DiscoveryGift
/// </summary>
public class DiscoveryGiftEvent : EventTranscript
{
    private readonly string _text;
    private readonly string[] _arguments;

    public DiscoveryGiftEvent(string text, string[] arguments)
    {
        _text = text;
        _arguments = arguments;
    }

    /// <summary>
    /// Finds a random item and add the discovery trait information for the given player.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="player"></param>
    /// <param name="argument"></param>
    /// <param name="text"></param>
    public override Task ExecuteAsync(Context context)
    {
        if (context.Player is null)
        {
            throw new ArgumentException($"event {nameof(DiscoveryGiftEvent)} is only allowed to be executed in context of a player", nameof(context));
        }

        string action = _arguments[0];

        switch (action)
        {
            case "SinglePlanet":
                DiscoverSinglePlanet(context, context.Player.Id, _text);
                break;
            case "SingleFlight":
                DiscoverFlight(context, context.Player.Id, _text);
                break;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Discovers a single flight (randomly)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="playerId"></param>
    /// <param name="text"></param>
    private static void DiscoverFlight(Context context, Guid playerId, string text)
    {
        var vectorShip = context.ActorContext.AsQueryable()
            .OfType<VectorShip>()
            .With((Locatable l) => l.HasOwnPosition)
            .With((Associatable a) => a.PlayerId != playerId)
            .TakeOneRandom();

        if (vectorShip != null)
        {
            vectorShip.Trait<Discoverable>().AddDiscoverer(playerId, DiscoveryLevel.PropertyAware, context.TurnContext.Turn);

            context.PlayerContext.SendMessageToPlayer(playerId, context.TurnContext.Turn,
                text: string.Format(text, vectorShip.TryGetTrait<Nameable>(out var nameable) ? nameable.Name : vectorShip.GetType().Name)
            );
        }
    }

    /// <summary>
    /// Discovers a single planet (randomly)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="playerId"></param>
    /// <param name="text"></param>
    private static void DiscoverSinglePlanet(Context context, Guid playerId, string text)
    {
        var planet = context.ActorContext.AsQueryable()
            .OfType<Planet>()
            .With((Associatable a) => a.PlayerId != playerId)
            .With((Discoverable d) => d.IsDiscoveredBy(playerId, DiscoveryLevel.LocationAware))
            .TakeOneRandom();

        if (planet != null)
        {
            planet.Trait<Discoverable>().AddDiscoverer(playerId, DiscoveryLevel.PropertyAware, context.TurnContext.Turn);

            context.PlayerContext.SendMessageToPlayer(playerId, context.TurnContext.Turn,
                text: string.Format(text, planet.Trait<Nameable>().Name)
            );
        }
    }
}
