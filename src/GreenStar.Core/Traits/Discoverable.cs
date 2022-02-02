using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar.Core.Traits;

public class Discoverable : Trait
{
    private readonly List<DiscoveryEntry> _discoverer = new List<DiscoveryEntry>();

    public int[] RequiredTurnSnapshots
        => _discoverer.Select(entry => entry.Turn).Distinct().OrderBy(x => x).ToArray();

    public bool IsDiscoveredBy(Guid playerId, DiscoveryLevel minimumLevel)
        => _discoverer.Any(x => (x.PlayerId == playerId || x.PlayerId == Guid.Empty) && x.Level >= minimumLevel);

    public DiscoveryLevel RetrieveDiscoveryLevel(Guid playerId)
        => RetrieveDiscoveryEntry(playerId)?.Level ?? DiscoveryLevel.Unknown;

    public DiscoveryEntry? RetrieveDiscoveryEntry(Guid playerId)
        => _discoverer.Where(x => (x.PlayerId == playerId || x.PlayerId == Guid.Empty))
            .OrderByDescending(x => x.Level)
            .FirstOrDefault();

    public void AddDiscoverer(Guid playerId, DiscoveryLevel level, int discoveryTurn)
    {
        var entry = _discoverer.FirstOrDefault(x => x.PlayerId == playerId);

        if (entry == null)
        {
            entry = new DiscoveryEntry()
            {
                PlayerId = playerId,
                Level = DiscoveryLevel.Unknown,
            };
            _discoverer.Add(entry);
        }

        var newLevel = (level > entry.Level) ? level : entry.Level; // do not forget the maximum knowledge state

        entry.Level = newLevel;
        entry.Turn = discoveryTurn;
    }
}
