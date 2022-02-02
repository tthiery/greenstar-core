using GreenStar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.Ships;

namespace GreenStar.Transcripts;

public class VectorFlightTurnTranscript : TraitTurnTranscript<VectorShip, VectorFlightCapable>
{
    public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
        => trait.UpdatePosition(context);
}
