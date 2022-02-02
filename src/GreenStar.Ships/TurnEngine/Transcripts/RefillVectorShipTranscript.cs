using GreenStar.Core.Traits;
using GreenStar.Ships;

namespace GreenStar.Core.TurnEngine.Transcripts;

public class RefillVectorShipTranscript : TraitTurnTranscript<VectorShip, VectorFlightCapable>
{
    public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
        => trait.TryRefillFuel(context);
}
