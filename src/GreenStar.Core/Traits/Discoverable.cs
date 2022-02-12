using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Persistence;

namespace GreenStar.Traits;

public class Discoverable : Trait
{
    public override void Load(IPersistenceReader reader)
    {
        const string PersistencePrefix = "Discoverer:";
        foreach (var propertyName in reader.ReadPropertyNames(PersistencePrefix))
        {
            var playerId = Guid.Parse(propertyName.Substring(PersistencePrefix.Length));
            var fields = reader.Read<string>(propertyName).Split("@");
            var level = Enum.Parse<DiscoveryLevel>(fields[0]);
            var turn = Convert.ToInt32(fields[1]);

            _discoverer.Add(new DiscoveryEntry() { PlayerId = playerId, Level = level, Turn = turn });
        }
    }
    public override void Persist(IPersistenceWriter writer)
    {
        foreach (var d in _discoverer)
        {
            writer.Write($"Discoverer:{d.PlayerId}", $"{d.Level.ToString()}@{d.Turn}");
        }
    }

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
