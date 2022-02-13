using System.Threading.Tasks;

using GreenStar;
using GreenStar.Ships;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class ColonizeTurnTranscript : TraitTurnTranscript<Ship, ColonizationCapable>
{
    public override Task ExecuteTraitAsync(Context context, Actor actor, ColonizationCapable trait)
    {
        trait.AutoColonizeOrRecruit(context);

        return Task.CompletedTask;
    }
}
