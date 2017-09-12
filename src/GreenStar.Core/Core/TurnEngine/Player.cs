using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar.Core.TurnEngine
{
    public abstract class Player
    {
        public Guid Id { get; }
        public string ColorCode { get; }
        public IEnumerable<Guid> SupportPlayers { get; }
        public int CompletedTurn { get; set; } = -1;

        public Player(Guid id, string colorCode, IEnumerable<Guid> supportPlayers)
        {
            if (string.IsNullOrWhiteSpace(colorCode))
            {
                throw new ArgumentException("message", nameof(colorCode));
            }

            Id = id;
            ColorCode = colorCode;
            SupportPlayers = supportPlayers ?? throw new ArgumentNullException(nameof(supportPlayers));
        }

        public bool IsFriendlyTo(Player other)
            => other == null ? false : IsFriendlyTo(other.Id);

        public bool IsFriendlyTo(Guid otherPlayerId)
            => this.Id == otherPlayerId || SupportPlayers.Any(p => p == otherPlayerId);
    }
}