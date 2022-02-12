using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.Events;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

/// <summary>
/// Applies randomly events to players.
/// </summary>
public class RandomEventsTurnTranscript : TurnTranscript
{

    private List<RandomEvent> _events = new List<RandomEvent>();
    private List<RandomEvent> _occuredEvents = new List<RandomEvent>();

    private readonly Random _random = new Random();
    private readonly IRandomEventsLoader _randomEventsLoader;
    private bool _loaded;

    public RandomEventsTurnTranscript(IRandomEventsLoader randomEventsLoader)
    {
        _randomEventsLoader = randomEventsLoader ?? throw new ArgumentNullException(nameof(randomEventsLoader));
    }

    /// <summary>
    /// On each iteration determine wether a random event should occur.
    /// </summary>
    /// <remarks>
    /// First, identify whether a random event should occur, then randomly to whom, then check whether valid for him
    /// </remarks>
    public override async Task ExecuteAsync(Context context)
    {
        await LoadRandomEventsAsync();

        await ExecuteRandomEventsAsync(context);
    }

    /// <summary>
    /// Loads all random events from the configuration file
    /// </summary>
    private async Task LoadRandomEventsAsync()
    {
        if (_loaded == false)
        {
            _occuredEvents = new List<RandomEvent>();

            var load = await _randomEventsLoader.LoadRandomEventsAsync();

            _events = new List<RandomEvent>(load);

            _loaded = true;
        }
    }

    /// <summary>
    /// Find and execute a random event
    /// </summary>
    private async Task ExecuteRandomEventsAsync(Context context)
    {
        var players = context.PlayerContext.GetAllPlayers();

        var playerCount = players.Count();

        if (playerCount > 0)
        {

            foreach (RandomEvent ev in _events.OrderBy(x => _random.Next()))
            {
                double number = _random.NextDouble();
                double index = 1.0f / ev.Prohability;

                if (index > number)
                {
                    int playerIdx = _random.Next(0, playerCount - 1);

                    var player = players.ElementAt(playerIdx);

                    if (ev.IsReturning || _occuredEvents.Contains(ev) == false)
                    {
                        await ApplyEventToPlayerAsync(context, ev, player);

                        if (ev.IsReturning == false)
                        {
                            _occuredEvents.Add(ev);
                        }

                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Apply a certain event to a player if suitable for him
    /// </summary>
    /// <param name="ev"></param>
    /// <param name="player"></param>
    public async Task ApplyEventToPlayerAsync(Context context, RandomEvent ev, Player player)
    {
        if (ev.RequiredTechnologies == null || ev.RequiredTechnologies.All(x => player.Capable.Of(x) > 0))
        {
            if (ev.BlockingTechnologies == null || !ev.BlockingTechnologies.Any(x => player.Capable.Of(x) > 0))
            {
                await context.TurnContext.ExecuteEventAsync(context, player, ev.Type, ev.Arguments, ev.Text);
            }
        }
    }
}
