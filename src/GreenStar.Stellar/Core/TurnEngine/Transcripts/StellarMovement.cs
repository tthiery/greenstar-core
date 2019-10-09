using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class StellarMovement : TraitTurnTranscript<StellarMoving>
    {
        public override void ExecuteTrait(Context context, Actor actor, StellarMoving trait)
            => trait.Move(context);
    }
}