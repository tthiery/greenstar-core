using GreenStar.Traits;
using GreenStar.Ships;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class RefillVectorShipTurnTranscript : TraitTurnTranscript<VectorShip, VectorFlightCapable>
{
    public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
        => trait.TryRefillFuel(context);
}
