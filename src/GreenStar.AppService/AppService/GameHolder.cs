using GreenStar.TurnEngine;

namespace GreenStar.AppService;

internal static class GameHolder
{
    public static Dictionary<Guid, TurnManager> Games { get; } = new();
}
