using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddEventTranscripts(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new RandomEventsTurnTranscript());

        return self;
    }
}
