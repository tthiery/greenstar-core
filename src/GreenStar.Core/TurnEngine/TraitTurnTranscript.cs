using System.Linq;

namespace GreenStar.Core.TurnEngine;

public abstract class TraitTurnTranscript<TActor, TTrait> : TurnTranscript
    where TActor : Actor
    where TTrait : Trait
{
    public override void Execute(Context context)
    {
        foreach (var actor in context.ActorContext.GetActors<TActor, TTrait>())
        {
            var trait = actor.Trait<TTrait>();

            ExecuteTrait(context, actor, trait);
        }
    }

    public abstract void ExecuteTrait(Context context, Actor actor, TTrait trait);
}
