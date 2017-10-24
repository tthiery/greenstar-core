using System;

namespace GreenStar.Core.TurnEngine.Players
{
    public class SystemPlayer : Player
    {
        public SystemPlayer()
            : base(Guid.Empty, "White", Array.Empty<Guid>(), 0, 0)
        {
        }
    }
}