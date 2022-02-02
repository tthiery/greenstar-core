using GreenStar.Core.TurnEngine.Transcripts;
using GreenStar.Research;

namespace GreenStar.Core.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self, ResearchProgressEngine progressEngine, IPlayerTechnologyStateLoader stateLoader)
    {
        self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new ResearchTranscript(progressEngine, stateLoader));

        return self;
    }
}
