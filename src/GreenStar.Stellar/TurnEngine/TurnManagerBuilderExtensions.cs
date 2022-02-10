using GreenStar.Stellar;
using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript<StellarMovementTurnTranscript<Sun>>(TurnTranscriptGroups.UniverseLife); // first move suns
        self.AddTranscript<StellarMovementTurnTranscript<Planet>>(TurnTranscriptGroups.UniverseLife); // then move planets
        self.AddTranscript<PopulationLifeTurnTranscript>(TurnTranscriptGroups.UniverseLife);
        self.AddTranscript<CalculateResourceRevenuesTurnTranscripts>(TurnTranscriptGroups.UnverseLifeAfterUnrest);

        return self;
    }
}
