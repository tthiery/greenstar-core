using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GreenStar.Core.TurnEngine;

public class InMemoryGame : IPlayerContext, IActorContext, ITurnContext
{
    private ImmutableList<Actor> _actors;
    public InMemoryGame(Guid id, IEnumerable<Player> players, IEnumerable<Actor> actors)
    {
        Id = id;
        Players = players ?? throw new ArgumentNullException(nameof(players));
        _actors = actors?.ToImmutableList() ?? throw new ArgumentNullException(nameof(actors));
    }

    public Guid Id { get; }
    public int Turn { get; set; } = 0;
    public IEnumerable<Player> Players { get; }
    public IEnumerable<Actor> Actors { get => _actors; }

    public Actor? GetActor(Guid actorId)
        => Actors.FirstOrDefault(a => a.Id == actorId);

    public void AddActor(Actor actor)
        => _actors = _actors.Add(actor);

    public void RemoveActor(Actor actor)
        => _actors = _actors.Remove(actor);

    public IQueryable<Actor> AsQueryable()
        => _actors.AsQueryable();

    private List<Message> _messages = new();

    public void SendMessageToPlayer(Guid playerId, int turnId, string type = "Info", string? text = null, object? data = null)
    {
        _messages.Add(new Message(playerId, turnId, type, text));
    }

    public IEnumerable<Message> GetMessagesByPlayer(Guid playerId, int minimumTurnId)
        => _messages.Where(m => (m.PlayerId == Guid.Empty || m.PlayerId == playerId) && m.Turn >= minimumTurnId);

    public Player? GetPlayer(Guid playerId)
        => this.Players.FirstOrDefault(p => p.Id == playerId);

    public IEnumerable<Player> GetAllPlayers()
        => Players;
}

public record Message(Guid PlayerId, int Turn, string MessageType, string Text);