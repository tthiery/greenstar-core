using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddEventTranscripts(this TurnManagerBuilder self)
    {
        self.AddTranscript<RandomEventsTurnTranscript>(TurnTranscriptGroups.UniverseLife);

        return self;
    }
}
