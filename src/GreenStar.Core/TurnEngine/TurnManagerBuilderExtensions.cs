using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddCoreTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript<CreateTurnBillingTurnTranscript>(TurnTranscriptGroups.InitTurn);
        self.AddTranscript<ClearTurnBillingTurnTranscript>(TurnTranscriptGroups.EndTurn);

        return self;
    }
}
