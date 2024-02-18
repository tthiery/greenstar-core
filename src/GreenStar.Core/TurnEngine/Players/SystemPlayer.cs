using System;

namespace GreenStar.TurnEngine.Players;

public class SystemPlayer : Player
{
    public static readonly Guid SystemPlayerId = Guid.Empty;
    public SystemPlayer()
    {
        Id = SystemPlayerId;
        Relatable.ColorCode = "White";
        IdealConditions.IdealGravity = 0;
        IdealConditions.IdealTemperature = 0;
    }
}
