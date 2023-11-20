using System;
using System.Threading.Tasks;

using GreenStar.Persistence;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class LoadFromPersistenceSetup : SetupTranscript
{
    private readonly IPersistence _persistence;

    public LoadFromPersistenceSetup(IPersistence persistence)
    {
        _persistence = persistence ?? throw new System.ArgumentNullException(nameof(persistence));
    }

    public override async Task ExecuteAsync(Context context)
    {
        var (turn, players, actors) = await _persistence.LoadFullAsync();
        var playerImplementation = context.PlayerContext as InMemoryPlayerStore ?? throw new InvalidOperationException();

        foreach (var p in players)
        {
            playerImplementation.SetupPlayer(p);
        }
        foreach (var a in actors)
        {
            context.ActorContext.AddActor(a);
        }

        if (context.TurnContext is TurnContext implementation)
        {
            implementation.Turn = turn;
        }
    }
}