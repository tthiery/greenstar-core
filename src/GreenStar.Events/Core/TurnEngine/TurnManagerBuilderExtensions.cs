using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Core.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddEventTranscripts(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new RandomEvents());

        return self;
    }
}
