using GreenStar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.Ships;

namespace GreenStar.TurnEngine.Transcripts;

public class ColonizeTranscript : TraitTurnTranscript<Ship, ColonizationCapable>
{
    public override void ExecuteTrait(Context context, Actor actor, ColonizationCapable trait)
        => trait.AutoColonizeOrRecruit(context);
}
