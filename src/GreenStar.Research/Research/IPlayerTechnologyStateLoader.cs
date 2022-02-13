using System;
using System.Threading.Tasks;

namespace GreenStar.Research;

public interface IPlayerTechnologyStateLoader
{
    Task<PlayerTechnologyState> LoadAsync(Guid playerId);
    Task SaveAsync(Guid playerId, PlayerTechnologyState state);
}
