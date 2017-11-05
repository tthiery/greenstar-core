using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class RefillVectorShipTranscript : TraitTurnTranscript<VectorFlightCapable>
    {
        public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
            => trait.TryRefillFuel(context);
    }
}