using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Ships
{
    public static class TurnManagerBuilderExtensions
    {
        public static TurnManagerBuilder AddElementsTranscript(this TurnManagerBuilder self)
        {
            self.AddTranscript(new VectorFlightTranscript());
            self.AddTranscript(new RefillVectorShipTranscript());

            return self;
        }
    }
}