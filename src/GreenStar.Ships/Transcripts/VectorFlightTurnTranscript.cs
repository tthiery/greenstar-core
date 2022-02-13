using System.Threading.Tasks;

using GreenStar;
using GreenStar.Algorithms;
using GreenStar.Ships;
using GreenStar.Traits;
using GreenStar.TurnEngine;

using Microsoft.Extensions.Options;

namespace GreenStar.Transcripts;

public class VectorFlightTurnTranscript : TraitTurnTranscript<VectorShip, VectorFlightCapable>
{
    private readonly IOptions<ResearchOptions> _options;

    public VectorFlightTurnTranscript(IOptions<ResearchOptions> options)
    {
        _options = options;
    }

    public override Task ExecuteTraitAsync(Context context, Actor actor, VectorFlightCapable trait)
    {
        trait.UpdatePosition(context, _options.Value);

        return Task.CompletedTask;
    }
}
