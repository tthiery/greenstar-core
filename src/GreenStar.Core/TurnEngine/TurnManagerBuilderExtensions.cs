using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddCoreTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.InitTurn, new CreateTurnBillingTurnTranscript());
        self.AddTranscript(TurnTranscriptGroups.EndTurn, new ClearTurnBillingTurnTranscript());

        return self;
    }
}
