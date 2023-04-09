using System;
using System.Collections.Generic;

using GreenStar.TurnEngine.Players;

namespace GreenStar.Test;

public static class Helper
{
    public static HumanPlayer CreateHumanPlayer(Guid id, string colorCode, IEnumerable<Guid> supportedPlayers, double idealTemperature, double idealGravity)
    {
        var player = new HumanPlayer(id);
        player.Relatable.PlayerId = player.Id;
        player.Relatable.ColorCode = colorCode;
        player.Relatable.SupportPlayers = supportedPlayers;
        player.IdealConditions.IdealTemperature = idealTemperature;
        player.IdealConditions.IdealGravity = idealGravity;

        return player;
    }
}