using GreenStar.Core;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class VectorFlightTranscript : TraitTurnTranscript<VectorFlightCapable>
    {
        public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
            => trait.UpdatePosition(context);
    }
}