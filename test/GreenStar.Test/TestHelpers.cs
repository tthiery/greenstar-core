using GreenStar.Core.TurnEngine;

namespace GreenStar;

public static class TestHelpers
{
    public static void FinishTurnForAllPlayers(this TurnManager self)
    {
        foreach (var player in self.Players.GetAllPlayers())
        {
            self.FinishTurn(player.Id);
        }
    }
}
