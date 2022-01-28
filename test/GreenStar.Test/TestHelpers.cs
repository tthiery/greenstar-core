using GreenStar.Core.TurnEngine;

namespace GreenStar;

public static class TestHelpers
{
    public static void FinishTurnForAllPlayers(this TurnManager self)
    {
        foreach (var player in self.Game.Players)
        {
            self.FinishTurn(player.Id);
        }
    }
}
