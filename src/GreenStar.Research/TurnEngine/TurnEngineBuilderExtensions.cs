using GreenStar.TurnEngine.Transcripts;
using GreenStar.Research;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self, ResearchProgressEngine progressEngine, IPlayerTechnologyStateLoader stateLoader)
    {
        self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new ResearchTranscript(progressEngine, stateLoader));

        return self;
    }
}
