namespace GreenStar.AppService.Actor;

public interface ICommandService
{
    Task ExecuteCommandAsync(Guid gameId, Guid playerId, Command requestedCommand);

    IEnumerable<Command> GetAllCommands(Guid gameId, Guid playerId, Guid actorId);
}
