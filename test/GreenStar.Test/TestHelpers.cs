using System.Threading.Tasks;

using GreenStar.TurnEngine;

namespace GreenStar;

public static class TestHelpers
{
    public static async Task FinishTurnForAllPlayersAsync(this TurnManager self)
    {
        foreach (var player in self.Players.GetAllPlayers())
        {
            await self.FinishTurnAsync(player.Id);
        }
    }
}
