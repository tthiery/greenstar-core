using GreenStar.Stellar;
using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new StellarMovementTurnTranscript<Sun>()); // first move suns
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new StellarMovementTurnTranscript<Planet>()); // then move planets
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new PopulationLifeTurnTranscript());
        self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new CalculateResourceRevenuesTurnTranscripts());

        return self;
    }
}
