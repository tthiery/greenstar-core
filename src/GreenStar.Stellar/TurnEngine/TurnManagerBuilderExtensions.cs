using GreenStar.Core.TurnEngine.Transcripts;
using GreenStar.Stellar;

namespace GreenStar.Core.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new StellarMovement<Sun>()); // first move suns
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new StellarMovement<Planet>()); // then move planets
        self.AddTranscript(TurnTranscriptGroups.UniverseLife, new PopulationLife());
        self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new CalculateResourceRevenues());

        return self;
    }
}
