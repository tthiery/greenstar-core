namespace GreenStar.AppService.Actor;

public class CommandDomainService : ICommandService
{
    public async Task ExecuteCommandAsync(Guid gameId, Guid playerId, Command requestedCommand)
    {
        var turnManager = GameHolder.Games[gameId];

        var context = turnManager.CreateTurnContext(playerId);

        if (context is not null && context.Player is not null)
        {
            var actor = context.ActorContext.GetActor(requestedCommand.ActorId);

            var command = actor?.GetCommands().FirstOrDefault(c => c.Id == requestedCommand.Id);

            if (command is not null)
            {
                command = command with
                {
                    Arguments = requestedCommand.Arguments
                };
                await context.TurnContext.ExecuteCommandAsync(context, context.Player, command);
            }
        }
    }

    public IEnumerable<Command> GetAllCommands(Guid gameId, Guid playerId, Guid actorId)
    {
        var turnManager = GameHolder.Games[gameId];

        var actor = turnManager.Actors.GetActor(actorId);

        if (actor is not null)
        {
            var commands = actor.GetCommands();

            return commands.Select(c => new Command(c.Id, c.Title, c.ActorId, c.Arguments));
        }
        else
        {
            return Array.Empty<Command>();
        }
    }
}