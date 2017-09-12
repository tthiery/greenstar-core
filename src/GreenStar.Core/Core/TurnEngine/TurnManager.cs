using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GreenStar.Core.TurnEngine
{
    public class TurnManager
    {
        public TurnManager(Game game, IEnumerable<TurnTranscript> transcripts)
        {
            Game = game ?? throw new System.ArgumentNullException(nameof(game));
            Transcripts = transcripts ?? throw new ArgumentNullException(nameof(transcripts));
        }

        public Game Game { get; }
        public IEnumerable<TurnTranscript> Transcripts { get; }

        public bool IsTurnOpenForPlayer(Guid playerId)
            => Game.Players.Any(p => p.Id == playerId && p.CompletedTurn < Turn);

        public void FinishTurn(Guid playerId)
        {
            if (IsTurnOpenForPlayer(playerId))
            {
                var player = Game.Players.FirstOrDefault(x => x.Id == playerId);

                player.CompletedTurn = Turn;
            }

            CheckAndStartRound();
        }

        private void CheckAndStartRound()
        {
            if (Game.Players.All(x => x.CompletedTurn == Turn))
            {
                StartRound();
            }
        }

        public int Turn { get; private set; } = 0;

        public void StartRound()
        {
            Turn++;

            /*
            if (this.Messages == null)
            {
                throw new InvalidOperationException("Game.Messages is not set");
            }

            Turn++;

            Messages.Add(new InfoMessage()
            {
                PlayerId = Guid.Empty,
                MessageType = "Info",
                Text = string.Format(CultureInfo.InvariantCulture, "The year {0} started", this.Year),
                Year = this.Year,
            });
            */

            Trace.WriteLine("Start Turn " + Turn.ToString());

            Stopwatch watch = new Stopwatch();
            long lastEllapsedMilliseconds = 0;
            watch.Start();

            Dictionary<string, object> intermediateData = new Dictionary<string, object>();

            foreach (TurnTranscript trans in Transcripts)
            {
                Trace.WriteLine("Execute " + trans.GetType().ToString());

                trans.IntermediateData = intermediateData;
                trans.Game = this.Game;

                trans.Execute();

                long deltaMilliseconds = watch.ElapsedMilliseconds - lastEllapsedMilliseconds;
                lastEllapsedMilliseconds = watch.ElapsedMilliseconds;
                Trace.WriteLine(" + Duration " + deltaMilliseconds);
            }

            watch.Stop();

            Trace.WriteLine("End Turn " + Turn.ToString() + " in " + watch.ElapsedMilliseconds);
        }
    }
}