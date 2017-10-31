using System.Collections.Generic;

namespace GreenStar.Core.TurnEngine
{
    public abstract class TurnTranscript
    {
        public abstract void Execute();

        public Dictionary<string, object> IntermediateData { get; set; }
        public InMemoryGame Game { get; set; }

        public IPlayerContext PlayerContext
            => Game;
        public IActorContext ActorContext
            => Game;
        public ITurnContext TurnContext
            => Game;
    }
}