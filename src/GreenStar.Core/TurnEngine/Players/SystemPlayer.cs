using System;

namespace GreenStar.TurnEngine.Players;

public class SystemPlayer : Player
{
    public static readonly Guid SystemPlayerId = Guid.Empty;
    public SystemPlayer()
        : base(SystemPlayerId, "White", Array.Empty<Guid>(), 0, 0)
    {
    }
}
