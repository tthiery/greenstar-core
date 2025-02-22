using GreenStar.Resources;

namespace GreenStar.AppService.Turn;

public record Information(int Turn, ResourceAmount Resources, Message[] NewMessages);
public record TurnCompleted(Guid GameId, int Turn);

public interface ITurnService
{
    Task Finish(Guid gameId, Guid playerId);
    Information Information(Guid gameId, Guid playerId);
    IObservable<TurnCompleted> TurnCompleted { get; }
}