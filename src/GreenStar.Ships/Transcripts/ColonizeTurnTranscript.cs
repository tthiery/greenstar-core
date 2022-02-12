using GreenStar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.Ships;
using System.Threading.Tasks;

namespace GreenStar.Transcripts;

public class ColonizeTurnTranscript : TraitTurnTranscript<Ship, ColonizationCapable>
{
    public override Task ExecuteTraitAsync(Context context, Actor actor, ColonizationCapable trait)
    {
        trait.AutoColonizeOrRecruit(context);

        return Task.CompletedTask;
    }
}
