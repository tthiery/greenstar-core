using System;
using System.Collections.Generic;

using GreenStar.Research;

namespace GreenStar.TurnEngine;

public class InMemoryPlayerTechnologyStateLoader : IPlayerTechnologyStateLoader
{
    private Dictionary<Guid, PlayerTechnologyState> _states = new();
    public PlayerTechnologyState Load(Guid playerId)
        => _states.TryGetValue(playerId, out var value) ? value : throw new ArgumentException("no state for player found");

    public void Save(Guid playerId, PlayerTechnologyState state)
        => _states[playerId] = state;
}