using System;
using System.Collections.Generic;
using System.Linq;
using GreenStar.Events;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    /// <summary>
    /// Applies randomly events to players.
    /// </summary>
    public class RandomEvents : TurnTranscript
    {

        private List<RandomEvent> events;
        private List<RandomEvent> occuredEvents;

        private Random _random;
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

            ExecuteRandomEvents(context, Game.Players);
        }

        /// <summary>
        /// Loads all random events from the configuration file
        /// </summary>
        private void LoadRandomEvents()
        {
            if (_loaded == false)
            {
                _random = new Random();
                occuredEvents = new List<RandomEvent>();

                events = null; //TODO

                _loaded = true;
            }
        }

        /// <summary>
        /// Find and execute a random event
        /// </summary>
        private void ExecuteRandomEvents(Context context, IEnumerable<Player> players)
        {
            if (players == null)
            {
                throw new System.ArgumentNullException(nameof(players));
            }

            var playerCount = players.Count();

            if (playerCount > 0)
            {
                double number = _random.NextDouble();

                foreach (RandomEvent ev in events.OrderBy(x => _random.Next()))
                {
                    double index = 1.0f / ev.Prohability;

                    if (index > number)
                    {
                        int playerIdx = _random.Next(0, playerCount - 1);

                        var player = players.ElementAt(playerIdx);

                        if (ev.IsReturning || occuredEvents.Contains(ev) == false)
                        {
                            ApplyEventToPlayer(context, ev, player);

                            if (ev.IsReturning == false)
                            {
                                occuredEvents.Add(ev);
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

                    Type t = Type.GetType("GreenStar.Events." + type + "EventExecutor, GreenStar.Events");

                    if (t != null)
                    {
                        var eventExecutor = Activator.CreateInstance(t) as IEventExecutor;

                        eventExecutor.Execute(context, player, ev.Argument, ev.Text);
                    }
                }
            }
        }
    }
}