using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenStar.TurnEngine;

public class InMemoryPlayerStore : IPlayerContext, IPlayerView
{
    private readonly List<Message> _messages = new();
    private IEnumerable<Player> _players { get; }

    public InMemoryPlayerStore(IEnumerable<Player> players)
    {
        _players = players ?? throw new ArgumentNullException(nameof(players));
    }


    public void SendMessageToPlayer(Guid playerId, int turnId, string type = "Info", string? text = null, object? data = null)
    {
        _messages.Add(new Message(playerId, turnId, type, text ?? string.Empty));
    }

    public IEnumerable<Message> GetMessagesByPlayer(Guid playerId, int minimumTurnId)
        => _messages.Where(m => (m.PlayerId == Guid.Empty || m.PlayerId == playerId) && m.Turn >= minimumTurnId);

    public Player? GetPlayer(Guid playerId)
        => this._players.FirstOrDefault(p => p.Id == playerId);

    public IEnumerable<Player> GetAllPlayers()
        => _players;
}
