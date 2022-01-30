using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using GreenStar.Core;

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
        _messages.Add(new Message(playerId, turnId, type, text ?? string.Empty));
    }

    public IEnumerable<Message> GetMessagesByPlayer(Guid playerId, int minimumTurnId)
        => _messages.Where(m => (m.PlayerId == Guid.Empty || m.PlayerId == playerId) && m.Turn >= minimumTurnId);

    public Player? GetPlayer(Guid playerId)
        => this.Players.FirstOrDefault(p => p.Id == playerId);

    public IEnumerable<Player> GetAllPlayers()
        => Players;

    public Actor? GetRandomActor(Func<Actor, bool> predicate)
        => _actors.Where(predicate).AsQueryable().TakeOneRandom();

    public IEnumerable<TActor> GetActors<TActor, TTrait>(Func<TTrait, bool>? predicate = null)
        where TActor : Actor
        where TTrait : Trait
        => AsQueryable()
            .Where(a =>
                a is TActor
                && a.HasTrait<TTrait>()
                && (predicate == null || predicate(a.Trait<TTrait>())))
            .Cast<TActor>();
}

public record Message(Guid PlayerId, int Turn, string MessageType, string Text);