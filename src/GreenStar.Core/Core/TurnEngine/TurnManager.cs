using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GreenStar.Core.TurnEngine;

public class TurnManager
{
    private readonly InMemoryPlayerStore _playerStore;
    private InMemoryActorStore _actorStore { get; }
    private readonly TurnContext _turn = new TurnContext();

    public TurnManager(InMemoryActorStore actorStore, InMemoryPlayerStore playerStore, IEnumerable<TurnTranscript> transcripts)
    {
        _actorStore = actorStore ?? throw new System.ArgumentNullException(nameof(actorStore));
        _playerStore = playerStore;
        Transcripts = transcripts.Where(t => t is not SetupTranscript).ToArray() ?? throw new ArgumentNullException(nameof(transcripts));
    }

    public IPlayerView Players => _playerStore;
    public IActorView Actors => _actorStore;
    public ITurnView Turn => _turn;

    public Context CreateTurnContext()
        => new Context(_turn, _playerStore, _actorStore);

    public TurnTranscript[] Transcripts { get; }

    public bool IsTurnOpenForPlayer(Guid playerId)
        => _playerStore.GetAllPlayers().Any(p => p.Id == playerId && p.CompletedTurn < _turn.Turn);

    public void FinishTurn(Guid playerId)
    {
        if (IsTurnOpenForPlayer(playerId))
        {
            var player = _playerStore.GetAllPlayers().FirstOrDefault(x => x.Id == playerId) ?? throw new InvalidOperationException("unknown player id");

            player.CompletedTurn = _turn.Turn;
        }

        CheckAndStartRound();
    }

    private void CheckAndStartRound()
    {
        if (_playerStore.GetAllPlayers().All(x => x.CompletedTurn == _turn.Turn))
        {
            StartRound();
        }
    }

    public void StartRound()
    {
        _turn.Turn++;

        int year = _turn.Turn * 10 + 2000;

        _playerStore.SendMessageToPlayer(Guid.Empty, _turn.Turn,
            text: $"The year {year} started"
        );

        Trace.WriteLine("Start Turn " + _turn.Turn.ToString());

        Stopwatch watch = new Stopwatch();
        long lastEllapsedMilliseconds = 0;
        watch.Start();

        Dictionary<string, object> intermediateData = new Dictionary<string, object>();

        foreach (var trans in Transcripts)
        {
            Trace.WriteLine("Execute " + trans.GetType().ToString());

            trans.IntermediateData = intermediateData;

            trans.Execute(CreateTurnContext());

            long deltaMilliseconds = watch.ElapsedMilliseconds - lastEllapsedMilliseconds;
            lastEllapsedMilliseconds = watch.ElapsedMilliseconds;
            Trace.WriteLine(" + Duration " + deltaMilliseconds);
        }

        watch.Stop();

        Trace.WriteLine("End Turn " + _turn.Turn.ToString() + " in " + watch.ElapsedMilliseconds);
    }
}
