using GreenStar.Traits;

namespace GreenStar.TurnEngine.Transcripts;

public class StellarMovement<TActor> : TraitTurnTranscript<TActor, StellarMoving>
    where TActor : Actor
{
    public override void ExecuteTrait(Context context, Actor actor, StellarMoving trait)
        => trait.Move(context);
}
