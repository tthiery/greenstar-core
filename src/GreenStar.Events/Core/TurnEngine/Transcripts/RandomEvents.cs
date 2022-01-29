using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Events;

namespace GreenStar.Core.TurnEngine.Transcripts;

/// <summary>
/// Applies randomly events to players.
/// </summary>
public class RandomEvents : TurnTranscript
{

    private List<RandomEvent> _events = new List<RandomEvent>();
    private List<RandomEvent> _occuredEvents = new List<RandomEvent>();

    private readonly Random _random = new Random();
    private bool _loaded;

    /// <summary>
    /// On each iteration determine wether a random event should occur.
    /// </summary>
    /// <remarks>
    /// First, identify whether a random event should occur, then randomly to whom, then check whether valid for him
    /// </remarks>
    public override void Execute(Context context)
    {
        LoadRandomEvents();

        ExecuteRandomEvents(context);
    }

    /// <summary>
    /// Loads all random events from the configuration file
    /// </summary>
    private void LoadRandomEvents()
    {
        if (_loaded == false)
        {
            _occuredEvents = new List<RandomEvent>();

            _events = new List<RandomEvent>(); //TODO

            _loaded = true;
        }
    }

    /// <summary>
    /// Find and execute a random event
    /// </summary>
    private void ExecuteRandomEvents(Context context)
    {
        IEnumerable<Player> players = context.PlayerContext.GetAllPlayers();
        if (players == null)
        {
            throw new System.ArgumentNullException(nameof(players));
        }

        var playerCount = players.Count();

        if (playerCount > 0)
        {
            double number = _random.NextDouble();

            foreach (RandomEvent ev in _events.OrderBy(x => _random.Next()))
            {
                double index = 1.0f / ev.Prohability;

                if (index > number)
                {
                    int playerIdx = _random.Next(0, playerCount - 1);

                    var player = players.ElementAt(playerIdx);

                    if (ev.IsReturning || _occuredEvents.Contains(ev) == false)
                    {
                        ApplyEventToPlayer(context, ev, player);

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
    public void ApplyEventToPlayer(Context context, RandomEvent ev, Player player)
    {
        if (ev == null)
        {
            throw new ArgumentNullException("ev");
        }
        if (player == null)
        {
            throw new ArgumentNullException("player");
        }
        if (player.Capable == null)
        {
            throw new ArgumentNullException("player.Capable");
        }

        if (ev.RequiredTechnologies == null || ev.RequiredTechnologies.All(x => player.Capable.Of(x) > 0))
        {
            if (ev.BlockingTechnologies == null || !ev.BlockingTechnologies.Any(x => player.Capable.Of(x) > 0))
            {
                string type = ev.Type;

                var t = Type.GetType("GreenStar.Events." + type + "EventExecutor, GreenStar.Events");

                if (t != null)
                {
                    var eventExecutor = Activator.CreateInstance(t) as IEventExecutor ?? throw new InvalidOperationException("lost type between calls?");

                    string[] args = ev.Argument.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    eventExecutor.Execute(context, player, ev.Text, args);
                }
            }
        }
    }
}
