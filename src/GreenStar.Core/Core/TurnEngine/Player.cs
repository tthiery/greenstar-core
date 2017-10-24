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

        public double IdealTemperature { get; }
        public double IdealGravity { get; }
        public int CompletedTurn { get; set; } = -1;

        public Player(Guid id, string colorCode, IEnumerable<Guid> supportPlayers, double idealTemperature, double idealGravity)
        {
            if (string.IsNullOrWhiteSpace(colorCode))
            {
                throw new ArgumentException("message", nameof(colorCode));
            }

            Id = id;
            ColorCode = colorCode;
            SupportPlayers = supportPlayers ?? throw new ArgumentNullException(nameof(supportPlayers));
            IdealTemperature = idealTemperature;
            IdealGravity = idealGravity;
        }

        public bool IsFriendlyTo(Player other)
            => other == null ? false : IsFriendlyTo(other.Id);

        public bool IsFriendlyTo(Guid otherPlayerId)
            => this.Id == otherPlayerId || SupportPlayers.Any(p => p == otherPlayerId);
    }
}