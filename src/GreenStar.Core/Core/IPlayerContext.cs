using System;
using System.Collections.Generic;

using GreenStar.Core.TurnEngine;

namespace GreenStar.Core;

public interface IPlayerContext
{
    void SendMessageToPlayer(Guid playerId, int turnId, string type = "Info", string? text = null, object? data = null);
    IEnumerable<Message> GetMessagesByPlayer(Guid playerId, int minimumTurnId);
    Player? GetPlayer(Guid playerId);
    IEnumerable<Player> GetAllPlayers();
}
