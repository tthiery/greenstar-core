using GreenStar.Traits;

namespace GreenStar.TurnEngine.Transcripts;

public class PopulationLife : TraitTurnTranscript<Actor, Populatable>
{
    public override void ExecuteTrait(Context context, Actor actor, Populatable trait)
        => trait.Life(context);
}
