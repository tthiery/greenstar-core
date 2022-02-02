using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class PopulationLifeTurnTranscript : TraitTurnTranscript<Actor, Populatable>
{
    public override void ExecuteTrait(Context context, Actor actor, Populatable trait)
        => trait.Life(context);
}
