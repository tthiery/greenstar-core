using System.Linq;

namespace GreenStar.Core.TurnEngine
{
    public abstract class TraitTurnTranscript<T> : TurnTranscript where T : Trait
    {
        public override void Execute(Context context)
        {
            foreach (var actor in Game.Actors.Where(a => a.HasTrait<T>()))
            {
                var trait = actor.Trait<T>();

                ExecuteTrait(context, actor, trait);
            }
        }

        public abstract void ExecuteTrait(Context context, Actor actor, T trait);
    }
}