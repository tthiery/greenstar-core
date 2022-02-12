using System.Threading.Tasks;

using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class StellarMovementTurnTranscript<TActor> : TraitTurnTranscript<TActor, StellarMoving>
    where TActor : Actor
{
    public override Task ExecuteTraitAsync(Context context, Actor actor, StellarMoving trait)
    {
        trait.Move(context);

        return Task.CompletedTask;
    }
}
