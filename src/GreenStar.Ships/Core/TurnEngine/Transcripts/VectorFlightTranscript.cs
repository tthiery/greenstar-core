using GreenStar.Core;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class VectorFlightTranscript : TraitTurnTranscript<VectorFlightCapableTrait>
    {
        public override void ExecuteTrait(Actor actor, VectorFlightCapableTrait trait)
            => trait.UpdatePosition(Game);
    }
}