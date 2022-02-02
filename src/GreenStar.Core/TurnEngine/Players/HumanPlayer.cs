using System;
using System.Collections.Generic;

namespace GreenStar.Core.TurnEngine.Players;

public class HumanPlayer : Player
{
    public HumanPlayer(Guid id, string colorCode, IEnumerable<Guid> supportPlayers, double idealTemperature, double idealGravity)
        : base(id, colorCode, supportPlayers, idealTemperature, idealGravity)
    {
    }
}
