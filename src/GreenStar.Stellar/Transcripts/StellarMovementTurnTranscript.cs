using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class StellarMovementTurnTranscript<TActor> : TraitTurnTranscript<TActor, StellarMoving>
    where TActor : Actor
{
    public override void ExecuteTrait(Context context, Actor actor, StellarMoving trait)
        => trait.Move(context);
}
