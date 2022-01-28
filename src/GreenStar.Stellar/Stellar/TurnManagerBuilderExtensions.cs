using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Stellar;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new StellarMovement());
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new PopulationLife());
        self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new CalculateResourceRevenues());

        return self;
    }
}
