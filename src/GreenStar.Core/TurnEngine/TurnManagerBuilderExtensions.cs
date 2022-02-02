using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Transcripts;

namespace GreenStar;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddCoreTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.InitTurn, new CreateTurnBillingTranscript());
        self.AddTranscript(TurnTranscriptGroups.EndTurn, new ClearTurnBillingTranscript());

        return self;
    }
}
