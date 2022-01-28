using GreenStar.Core;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class VectorFlightTranscript : TraitTurnTranscript<VectorFlightCapable>
    {
        public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
            => trait.UpdatePosition(context);
    }
}