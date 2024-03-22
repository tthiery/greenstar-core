using GreenStar.AppService.Turn;
using GreenStar.Resources;

using Spectre.Console;

namespace GreenStar.Cli;

public static class Turn
{
    public static async Task TurnCommandAsync(Guid gameId, Guid playerId)
    {
        if (gameId != Guid.Empty)
        {
            var turnFacade = new TurnDomainService();

            await turnFacade.Finish(gameId, playerId);

            var information = turnFacade.Information(gameId, playerId);

            AnsiConsole.WriteLine($"Turn {information.Turn} completed");
            AnsiConsole.WriteLine($"Resources: {information.Resources}");

            foreach (var msg in information.NewMessages)
            {
                AnsiConsole.WriteLine($"{msg.Turn}: {msg.Text}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("[red]No game started[/]");
        }
    }

    public static void LogCommand(Guid gameId, Guid playerId)
    {
        if (gameId != Guid.Empty)
        {
            var turnFacade = new TurnDomainService();

            var information = turnFacade.Information(gameId, playerId);

            foreach (var msg in information.NewMessages)
            {
                AnsiConsole.WriteLine($"{msg.Turn}: {msg.Text}");
            }
        }
        else
        {
            AnsiConsole.WriteLine("[red]No game started[/]");
        }
    }
}
