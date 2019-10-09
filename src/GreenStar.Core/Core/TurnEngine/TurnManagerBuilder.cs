using System;
using System.Collections.Generic;
using System.Linq;
using GreenStar.Core;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core.TurnEngine
{
    public class TurnManagerBuilder
    {
        private List<Player> _players;
        private List<Actor> _actors;
        private SortedList<int, TurnTranscript> _transcripts;

        public TurnManagerBuilder()
        {
            _players = new List<Player>();
            _actors = new List<Actor>();
            _transcripts = new SortedList<int, TurnTranscript>();
        }
        public TurnManager Build()
        {
            var game = new InMemoryGame(Guid.NewGuid(), _players, _actors);

            var turnEngine = new TurnManager(game, _transcripts.Select(kv => kv.Value));

            return turnEngine;
        }



        public TurnManagerBuilder AddPlayer(Player player)
        {
            _players.Add(player);

            return this;
        }

        public TurnManagerBuilder AddActor(Actor actor)
        {
            _actors.Add(actor);

            return this;
        }

        public TurnManagerBuilder AddTranscript(int group, TurnTranscript transcript)
        {
            var groupOffset = _transcripts.Count(kv => kv.Key >= group && kv.Key < group + 100);
            _transcripts.Add(group + groupOffset, transcript);

            return this;
        }
    }
}