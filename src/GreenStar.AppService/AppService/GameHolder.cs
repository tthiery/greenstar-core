using GreenStar.TurnEngine;

namespace GreenStar.AppService;

public static class GameHolder
{
    public static Dictionary<Guid, TurnManager> Games { get; } = new();
}
