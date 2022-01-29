using System;

namespace GreenStar.Research;

public interface IPlayerTechnologyStateLoader
{
    PlayerTechnologyState Load(Guid playerId);
    void Save(Guid playerId, PlayerTechnologyState state);
}
