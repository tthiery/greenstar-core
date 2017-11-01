using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Ships
{
    public static class TurnManagerBuilderExtensions
    {
        public static TurnManagerBuilder AddElementsTranscript(this TurnManagerBuilder self)
        {
            self.AddTranscript(TurnTranscriptGroups.Moves, new VectorFlightTranscript());
            self.AddTranscript(TurnTranscriptGroups.MovementDone, new RefillVectorShipTranscript());
            self.AddTranscript(TurnTranscriptGroups.UnverseLifeAfterUnrest, new ColonizeTranscript());

            return self;
        }
    }
}