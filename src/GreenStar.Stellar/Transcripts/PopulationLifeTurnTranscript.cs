using GreenStar.Algorithms;
using GreenStar.Traits;
using GreenStar.TurnEngine;

using Microsoft.Extensions.Options;

namespace GreenStar.Transcripts;

public class PopulationLifeTurnTranscript : TraitTurnTranscript<Actor, Populatable>
{
    private readonly IOptions<PlanetLifeOptions> _planetLifeOptions;

    public PopulationLifeTurnTranscript(IOptions<PlanetLifeOptions> planetLifeOptions)
    {
        _planetLifeOptions = planetLifeOptions;
    }
    public override void ExecuteTrait(Context context, Actor actor, Populatable trait)
        => trait.Life(context, _planetLifeOptions.Value);
}
