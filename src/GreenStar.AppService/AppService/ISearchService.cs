using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;

namespace GreenStar.AppService;

public record ShipRecord(Guid ActorId, string ShipType, string? LocationName, long X, long Y);

public interface ISearchService
{
    IEnumerable<Planet> GetAllKnownPlanets(Guid gameId, Guid playerId);
    IEnumerable<Planet> GetAllAssociatedPlanets(Guid gameId, Guid playerId);
    IEnumerable<ShipRecord> GetAllOwnedShips(Guid gameId, Guid playerId);
}

public class SearchDomainService : ISearchService
{
    public IEnumerable<Planet> GetAllKnownPlanets(Guid gameId, Guid playerId)
    {
        var turnManager = GameHolder.Games[gameId];

        var planets = turnManager.Actors.GetActors<Planet, Discoverable>(d => d.IsDiscoveredBy(playerId, DiscoveryLevel.LocationAware));

        return planets;
    }
    public IEnumerable<Planet> GetAllAssociatedPlanets(Guid gameId, Guid playerId)
    {
        var turnManager = GameHolder.Games[gameId];

        var planets = turnManager.Actors.GetActors<Planet, Associatable>(ass => ass.PlayerId == playerId);

        return planets;
    }

    public IEnumerable<ShipRecord> GetAllOwnedShips(Guid gameId, Guid playerId)
    {
        var turnManager = GameHolder.Games[gameId];

        var ships = turnManager.Actors.GetActors<Ship, Associatable>(ass => ass.PlayerId == playerId).ToArray();

        return ships.Select(p =>
        {
            var locationName = (p.Trait<Locatable>(), p.Trait<VectorFlightCapable>()) switch
            {
                ({ HasOwnPosition: true }, { ActiveFlight: true } and var vc) => $"In Flight to {turnManager.Actors.GetActor(vc.TargetActorId)?.Trait<Nameable>().Name}",
                ({ HasOwnPosition: false } and var l, _) => turnManager.Actors.GetActor(l.HostLocationActorId)?.Trait<Nameable>()?.Name,
            };
            var position = p.Trait<Locatable>().GetPosition(turnManager.Actors);

            return new ShipRecord(p.Id, p.GetType().Name, locationName, position.X, position.Y);
        });
    }
}
