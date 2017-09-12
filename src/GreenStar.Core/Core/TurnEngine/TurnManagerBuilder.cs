using System;
using System.Collections.Generic;
using GreenStar.Core;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core.TurnEngine
{
    public class TurnManagerBuilder
    {
        private List<Player> _players;
        private List<Actor> _actors;
        private List<TurnTranscript> _transcripts;

        public TurnManagerBuilder()
        {
            _players = new List<Player>();
            _actors = new List<Actor>();
            _transcripts = new List<TurnTranscript>();
        }
        public TurnManager Build()
        {
            var game = new Game(Guid.NewGuid(), _players, _actors);

            var turnEngine = new TurnManager(game, _transcripts);

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

        public TurnManagerBuilder AddTranscript(TurnTranscript transcript)
        {
            _transcripts.Add(transcript);

            return this;
        }
    }
}