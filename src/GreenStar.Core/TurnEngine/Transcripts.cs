using System.Collections.Generic;

namespace GreenStar.TurnEngine;

public abstract class Transcript
{
    public abstract void Execute(Context context);
}

public abstract class EventTranscript : Transcript
{ }

public abstract class TurnTranscript : Transcript
{
    public Dictionary<string, object> IntermediateData { get; set; } = new Dictionary<string, object>();
}

public abstract class SetupTranscript : TurnTranscript
{ }

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