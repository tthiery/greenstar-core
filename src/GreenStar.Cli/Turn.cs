using GreenStar.Core;
using GreenStar.Core.Resources;
using GreenStar.Core.TurnEngine;

using Spectre.Console;

namespace GreenStar.Cli;

public static class Turn
{
    public static void TurnCommand(Guid gameId, Guid playerId)
    {
        if (gameId != Guid.Empty)
        {
            var turnFacade = new TurnFacade();

            turnFacade.Finish(gameId, playerId);

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
            var turnFacade = new TurnFacade();

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

public record Information(int Turn, ResourceAmount Resources, Message[] NewMessages);

public class TurnFacade
{
    public void Finish(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            // TODO: for right now, finish all computers
            foreach (var player in turnManager.Players.GetAllPlayers())
            {
                turnManager.FinishTurn(player.Id);
            }
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public Information Information(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var playerView = turnManager.Players;

            var result = new Information(
                turnManager.Turn.Turn,
                playerView.GetPlayer(playerId).Resources,
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