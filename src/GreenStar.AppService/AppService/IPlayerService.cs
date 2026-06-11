using GreenStar.TurnEngine;

namespace GreenStar.AppService;

public interface IPlayerService
{
    IEnumerable<Player> GetAllPlayers(Guid gameId);
}

public class PlayerDomainService : IPlayerService
{
    public IEnumerable<Player> GetAllPlayers(Guid gameId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            return turnManager.Players.GetAllPlayers();
        }
        else
        {
            return [];
        }
    }
}