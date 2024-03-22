namespace GreenStar.AppService.Turn;

public class TurnDomainService : ITurnService
{
    public async Task Finish(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            // TODO: for right now, finish all computers
            foreach (var player in turnManager.Players.GetAllPlayers())
            {
                await turnManager.FinishTurnAsync(player.Id);
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    // this is not enough for a domain service
    public Information Information(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var playerView = turnManager.Players;

            var result = new Information(
                turnManager.Turn.Turn,
                playerView.GetPlayer(playerId)?.Resourceful?.Resources ?? string.Empty,
                playerView.GetMessagesByPlayer(playerId, turnManager.Turn.Turn).ToArray()
            );

            return result;
        }
        else
        {
            throw new InvalidOperationException("game not found");
        }
    }
}
