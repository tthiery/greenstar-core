using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GreenStar.Research;

namespace GreenStar.Research;

public class InMemoryPlayerTechnologyStateLoader : IPlayerTechnologyStateLoader
{
    private Dictionary<Guid, PlayerTechnologyState> _states = new();
    public Task<PlayerTechnologyState> LoadAsync(Guid playerId)
        => Task.FromResult(_states.TryGetValue(playerId, out var value) ? value : throw new ArgumentException("no state for player found"));

    public Task SaveAsync(Guid playerId, PlayerTechnologyState state)
    { _states[playerId] = state; return Task.CompletedTask; }
}