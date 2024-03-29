using System.Linq;
using System.Threading.Tasks;

using GreenStar.Ships.Factory;
using GreenStar.Stellar;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class InitialShipSetup : SetupTranscript
{
    private readonly ShipFactory _shipFactory;

    public InitialShipSetup(ShipFactory shipFactory)
    {
        _shipFactory = shipFactory;
    }

    public override async Task ExecuteAsync(Context context)
    {
        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            var homePlanets = context.ActorContext.GetActors<Planet, Associatable>(ass => ass.PlayerId == player.Id);

            var homePlanet = homePlanets.Last();

            foreach (var shipOrder in new[] {
                ("blueprint-0-scout", "Hope"),
                ("blueprint-0-scout", "Horizn"),
                ("blueprint-0-colonizeship", "Vertility"),
            })
            {
                var ship = await _shipFactory.CreateShipAsync(player.Id, shipOrder.Item1, shipOrder.Item2);

                ship.Trait<Associatable>().PlayerId = player.Id;
                homePlanet.Trait<Hospitality>().Enter(ship);

                context.ActorContext.AddActor(ship);
            }
        }
    }
}