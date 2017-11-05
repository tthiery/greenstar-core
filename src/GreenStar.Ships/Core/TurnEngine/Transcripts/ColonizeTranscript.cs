using GreenStar.Core;
using GreenStar.Core.TurnEngine;
using GreenStar.Core.Traits;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    public class ColonizeTranscript : TraitTurnTranscript<ColonizationCapable>
    {
        public override void ExecuteTrait(Context context, Actor actor, ColonizationCapable trait)
            => trait.AutoColonizeOrRecruit(context);
    }
}