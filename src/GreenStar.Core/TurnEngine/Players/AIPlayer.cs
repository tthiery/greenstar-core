using System;
using System.Collections.Generic;

namespace GreenStar.Core.TurnEngine.Players;

public class AIPlayer : Player
{
    public AIPlayer(Guid id, string colorCode, IEnumerable<Guid> supportPlayers, double idealTemperature, double idealGravity)
        : base(id, colorCode, supportPlayers, idealTemperature, idealGravity)
    {
    }
}