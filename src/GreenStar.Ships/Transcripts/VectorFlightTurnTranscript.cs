using GreenStar;
using GreenStar.Traits;
using GreenStar.TurnEngine;
using GreenStar.Ships;
using Microsoft.Extensions.Options;
using GreenStar.Algorithms;

namespace GreenStar.Transcripts;

public class VectorFlightTurnTranscript : TraitTurnTranscript<VectorShip, VectorFlightCapable>
{
    private readonly IOptions<ResearchOptions> _options;

    public VectorFlightTurnTranscript(IOptions<ResearchOptions> options)
    {
        _options = options;
    }

    public override void ExecuteTrait(Context context, Actor actor, VectorFlightCapable trait)
        => trait.UpdatePosition(context, _options.Value);
}
