using GreenStar.Core.TurnEngine;
using GreenStar.Core.TurnEngine.Transcripts;

namespace GreenStar.Stellar
{
    public static class TurnManagerBuilderExtensions
    {
        public static TurnManagerBuilder AddStellarTranscript(this TurnManagerBuilder self)
        {
            self.AddTranscript(new PopulationLife());

            return self;
        }
    }
}