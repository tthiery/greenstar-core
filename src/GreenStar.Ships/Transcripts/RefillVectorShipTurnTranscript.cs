using GreenStar.Traits;
using GreenStar.Ships;
using GreenStar.TurnEngine;
using System.Threading.Tasks;

namespace GreenStar.Transcripts;

public class RefillVectorShipTurnTranscript : TraitTurnTranscript<VectorShip, VectorFlightCapable>
{
    public override Task ExecuteTraitAsync(Context context, Actor actor, VectorFlightCapable trait)
    {
        trait.TryRefillFuel(context);

        return Task.CompletedTask;
    }
}
