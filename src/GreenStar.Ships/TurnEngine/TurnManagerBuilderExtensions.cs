using GreenStar.Transcripts;

namespace GreenStar.TurnEngine;

public static class TurnManagerBuilderExtensions
{
    public static TurnManagerBuilder AddElementsTranscript(this TurnManagerBuilder self)
    {
        self.AddTranscript(TurnTranscriptGroups.Moves, new VectorFlightTurnTranscript());
        self.AddTranscript(TurnTranscriptGroups.MovementDone, new RefillVectorShipTurnTranscript());
        self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new ColonizeTurnTranscript());

        return self;
    }
}
