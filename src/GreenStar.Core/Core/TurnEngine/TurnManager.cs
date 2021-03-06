using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GreenStar.Core.TurnEngine
{
    public class TurnManager
    {
        public TurnManager(InMemoryGame game, IEnumerable<TurnTranscript> transcripts)
        {
            Game = game ?? throw new System.ArgumentNullException(nameof(game));
            Transcripts = transcripts ?? throw new ArgumentNullException(nameof(transcripts));
        }

        public InMemoryGame Game { get; }

        public Context CreateTurnContext()
            => new Context(Game, Game, Game);

        public IEnumerable<TurnTranscript> Transcripts { get; }

        public bool IsTurnOpenForPlayer(Guid playerId)
            => Game.Players.Any(p => p.Id == playerId && p.CompletedTurn < Game.Turn);

        public void FinishTurn(Guid playerId)
        {
            if (IsTurnOpenForPlayer(playerId))
            {
                var player = Game.Players.FirstOrDefault(x => x.Id == playerId);

                player.CompletedTurn = Game.Turn;
            }

            CheckAndStartRound();
        }

        private void CheckAndStartRound()
        {
            if (Game.Players.All(x => x.CompletedTurn == Game.Turn))
            {
                StartRound();
            }
        }

        public void StartRound()
        {
            Game.Turn++;

            int year = Game.Turn * 10 + 2000;

            Game.SendMessageToPlayer(Guid.Empty,
                text: $"The year {year} started",
                year: year
            );

            Trace.WriteLine("Start Turn " + Game.Turn.ToString());

            Stopwatch watch = new Stopwatch();
            long lastEllapsedMilliseconds = 0;
            watch.Start();

            Dictionary<string, object> intermediateData = new Dictionary<string, object>();

            foreach (TurnTranscript trans in Transcripts)
            {
                Trace.WriteLine("Execute " + trans.GetType().ToString());

                trans.IntermediateData = intermediateData;
                trans.Game = this.Game;

                trans.Execute(new Context(this.Game, this.Game, this.Game));

                long deltaMilliseconds = watch.ElapsedMilliseconds - lastEllapsedMilliseconds;
                lastEllapsedMilliseconds = watch.ElapsedMilliseconds;
                Trace.WriteLine(" + Duration " + deltaMilliseconds);
            }

            watch.Stop();

            Trace.WriteLine("End Turn " + Game.Turn.ToString() + " in " + watch.ElapsedMilliseconds);
        }
    }
}