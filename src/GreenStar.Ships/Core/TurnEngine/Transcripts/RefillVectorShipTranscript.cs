using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class RefillVectorShipTranscript : TraitTurnTranscript<VectorFlightCapable>
    {
        public override void ExecuteTrait(Actor actor, VectorFlightCapable trait)
            => trait.TryRefillFuel(ActorContext, PlayerContext);
    }
}