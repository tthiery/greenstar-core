using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Core;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddCoreTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.InitTurn, new CreateTurnBillingTranscript());
        self.AddTranscript(TurnTranscriptGroups.EndTurn, new ClearTurnBillingTranscript());

        return self;
    }
}
