using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddElementsTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript<VectorFlightTurnTranscript>(TurnTranscriptGroups.Moves);
        self.AddTranscript<RefillVectorShipTurnTranscript>(TurnTranscriptGroups.MovementDone);
        self.AddTranscript<ColonizeTurnTranscript>(TurnTranscriptGroups.UnverseLifeAfterUnrest);

        return self;
    }
}
