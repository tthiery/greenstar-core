using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Persistence;
using GreenStar.TurnEngine;

namespace GreenStar.Traits;

public class Relatable : Trait
{
    public Guid PlayerId { get; set; } = Guid.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public IEnumerable<Guid> SupportPlayers { get; set; } = Array.Empty<Guid>();

    public bool IsFriendlyTo(Player other)
        => other == null ? false : IsFriendlyTo(other.Id);

    public bool IsFriendlyTo(Guid otherPlayerId)
        => PlayerId == otherPlayerId || SupportPlayers.Any(p => p == otherPlayerId);

    public override void Load(IPersistenceReader reader)
    {
        PlayerId = reader.Read<Guid>(nameof(PlayerId));
        ColorCode = reader.Read<string>(nameof(ColorCode));
        SupportPlayers = reader.Read<IEnumerable<Guid>>(nameof(SupportPlayers));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write<Guid>(nameof(PlayerId), PlayerId);
        writer.Write<string>(nameof(ColorCode), ColorCode);
        writer.Write<IEnumerable<Guid>>(nameof(SupportPlayers), SupportPlayers);
    }
}