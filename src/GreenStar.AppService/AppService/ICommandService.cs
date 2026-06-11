using System.Reactive.Subjects;

using GreenStar.Transcripts;

namespace GreenStar.AppService;

public interface ICommandService
{
    Task ExecuteCommandAsync(Guid gameId, Guid playerId, Command requestedCommand);

    IEnumerable<Command> GetAllCommands(Guid gameId, Guid playerId, Guid actorId);
    IObservable<Command> CommandCompleted { get; }
}

public class CommandDomainService : ICommandService
{

    private readonly Subject<Command> _commandCompleted = new();
    public IObservable<Command> CommandCompleted => _commandCompleted;

    public async Task ExecuteCommandAsync(Guid gameId, Guid playerId, Command requestedCommand)
    {
        var turnManager = GameHolder.Games[gameId];

        var context = turnManager.CreateTurnContext(playerId);

        if (context is not null && context.Player is not null)
        {
            var actor = context.ActorContext.GetActor(requestedCommand.ActorId);

            var command = requestedCommand switch
            {
                SetPrimarySelectionCommand => requestedCommand,
                _ => actor?.GetCommands().FirstOrDefault(c => c.Id == requestedCommand.Id)
            };

            if (command is not null)
            {
                command = command with
                {
                    Arguments = requestedCommand.Arguments
                };
                await context.TurnContext.ExecuteCommandAsync(context, context.Player, command);

                _commandCompleted.OnNext(requestedCommand);
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