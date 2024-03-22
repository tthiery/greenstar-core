using GreenStar.Ships;
using GreenStar.Stellar;

namespace GreenStar.AppService.Actor;

public record ShipRecord(Guid ActorId, string ShipType, string? LocationName, long X, long Y);

public interface ISearchService
{
    IEnumerable<Planet> GetAllKnownPlanets(Guid gameId, Guid playerId);
    IEnumerable<Planet> GetAllAssociatedPlanets(Guid gameId, Guid playerId);
    IEnumerable<ShipRecord> GetAllOwnedShips(Guid gameId, Guid playerId);
}