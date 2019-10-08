using System.Collections.Generic;

namespace GreenStar.Core.TurnEngine
{
    public abstract class TurnTranscript
    {
        public abstract void Execute(Context context);

        public Dictionary<string, object> IntermediateData { get; set; }
        public InMemoryGame Game { get; set; }
    }
}