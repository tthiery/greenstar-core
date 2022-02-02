using System;
using System.Collections.Generic;

using GreenStar.TurnEngine;

namespace GreenStar;

public record Message(Guid PlayerId, int Turn, string MessageType, string Text);

public interface IPlayerView
{
    IEnumerable<Player> GetAllPlayers();
    IEnumerable<Message> GetMessagesByPlayer(Guid playerId, int minimumTurnId);
    Player? GetPlayer(Guid playerId);
}

public interface IPlayerContext
{
    void SendMessageToPlayer(Guid playerId, int turnId, string type = "Info", string? text = null, object? data = null);
    IEnumerable<Message> GetMessagesByPlayer(Guid playerId, int minimumTurnId);
    Player? GetPlayer(Guid playerId);
    IEnumerable<Player> GetAllPlayers();
}
