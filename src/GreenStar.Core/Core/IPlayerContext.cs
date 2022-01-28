using System;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Core
{
    public interface IPlayerContext
    {
        void SendMessageToPlayer(Guid playerId, string type = "Info", string? text = null, int year = -1, object? data = null);
        Player? GetPlayer(Guid playerId);
    }
}