using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace GreenStar.AppService.Turn;

public class TurnDomainService : ITurnService
{
    private readonly Subject<TurnCompleted> _turnCompleted = new();
    public IObservable<TurnCompleted> TurnCompleted => _turnCompleted;

    public async Task Finish(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            // TODO: for right now, finish all computers
            foreach (var player in turnManager.Players.GetAllPlayers())
            {
                await turnManager.FinishTurnAsync(player.Id);
            }

            _turnCompleted.OnNext(new TurnCompleted(gameId, turnManager.Turn.Turn));
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
