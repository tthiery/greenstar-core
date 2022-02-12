
using Spectre.Console;

namespace GreenStar.Cli;

public static class CommandTerminal
{
    public static async Task CommandCommandAsync(Guid gameId, Guid playerId, List<Pick> picks)
    {
        var facade = new CommandFacade();

        var commandeerPick = AnsiConsole.Prompt(
            new SelectionPrompt<Pick>()
                .Title("Which item you want to [green]commad[/]?")
                .MoreChoicesText("[gray]more choices[/]")
                .UseConverter(p => p.Title)
                .AddChoices(picks)
        );

        AnsiConsole.WriteLine($"Actor: {commandeerPick.Title}");

        var commands = facade.GetAllCommands(gameId, playerId, commandeerPick.ActorId);

        var selectedCommand = AnsiConsole.Prompt(
            new SelectionPrompt<Command>()
                .Title("What [green]command[/] to select!")
                .MoreChoicesText("[gray]more choices[/]")
                .UseConverter(p => $"{p.Title}")
                .AddChoices(new[] { new IdleCommandResult() }.Union(commands)));

        AnsiConsole.WriteLine($"Command: {selectedCommand.Title}");

        if (selectedCommand is not IdleCommandResult)
        {
            for (int idx = 0; idx < selectedCommand.Arguments.Length; idx++)
            {
                var arg = selectedCommand.Arguments[idx];
                switch (arg.DataType)
                {
                    case CommandArgumentDataType.LocatableAndHospitableReference:
                        var argumentPick = AnsiConsole.Prompt<Pick>(
                            new SelectionPrompt<Pick>()
                                .Title($"Argument [green]{arg.Name}[/] What pick to select?")
                                .MoreChoicesText("[gray]more choices[/]")
                                .UseConverter(p => p.Title)
                                .AddChoices(picks.Where(p => p.DataType == arg.DataType)));

                        selectedCommand.Arguments[idx] = selectedCommand.Arguments[idx] with { Value = argumentPick.ActorId.ToString(), };

                        AnsiConsole.MarkupLine($"[green]{arg.Name}[/]: {argumentPick.Title}");
                        break;
                }
            }

            await facade.ExecuteCommandAsync(gameId, playerId, selectedCommand);
        }
    }
}


public record IdleCommandResult()
    : Command("idle", "Do Nothing", Guid.Empty, Array.Empty<CommandArgument>());

public class CommandFacade
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