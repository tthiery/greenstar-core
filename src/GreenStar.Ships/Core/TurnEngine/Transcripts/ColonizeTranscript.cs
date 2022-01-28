using GreenStar.Core;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core.TurnEngine.Transcripts;

public class ColonizeTranscript : TraitTurnTranscript<ColonizationCapable>
{
    public override void ExecuteTrait(Context context, Actor actor, ColonizationCapable trait)
        => trait.AutoColonizeOrRecruit(context);
}
