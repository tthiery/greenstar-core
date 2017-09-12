using System.Collections.Generic;

namespace GreenStar.Core.TurnEngine
{
    public abstract class TurnTranscript
    {
        public abstract void Execute();

        public Dictionary<string, object> IntermediateData { get; set; }
        public Game Game { get; set; }
    }
}