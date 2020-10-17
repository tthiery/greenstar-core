using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Events
{
    public static class TurnManagerBuilderExtensions
    {
        public static TurnManagerBuilder AddEventTranscripts(this TurnManagerBuilder self)
        {
            self.AddTranscript(TurnTranscriptGroups.UniverseLife, new RandomEvents());

            return self;
        }
    }
}