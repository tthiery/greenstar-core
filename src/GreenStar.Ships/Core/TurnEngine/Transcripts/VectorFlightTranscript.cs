using GreenStar.Core;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class VectorFlightTranscript : TraitTurnTranscript<VectorFlightCapable>
    {
        public override void ExecuteTrait(Actor actor, VectorFlightCapable trait)
            => trait.UpdatePosition(ActorContext, TurnContext);
    }
}