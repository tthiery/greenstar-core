using GreenStar.Research;
using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddResearchTranscripts(this TurnManagerBuilder self)
        => self
            .AddTranscript<TechnologySetup>(TurnTranscriptGroups.Setup)
            .AddTranscript<ResearchTurnTranscript>(TurnTranscriptGroups.UnverseLifeAfterUnrest)
            .AddTranscript<AdjustResearchBudgetTurnTranscript>(TurnTranscriptGroups.EndAdministration);
}
