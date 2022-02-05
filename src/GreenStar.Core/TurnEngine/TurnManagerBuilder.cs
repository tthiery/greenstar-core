using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

namespace GreenStar.TurnEngine;

public class TurnManagerBuilder
{
    private readonly List<Player> _players;
    private readonly List<Actor> _actors;
    private readonly SortedList<int, TurnTranscript> _transcripts;

    public TurnManagerBuilder()
    {
        _players = new List<Player>();
        _actors = new List<Actor>();
        _transcripts = new SortedList<int, TurnTranscript>();
    }
    public TurnManager Build()
    {
        var game = new InMemoryActorStore(Guid.NewGuid(), _actors);

        var turnEngine = new TurnManager(game, new InMemoryPlayerStore(_players), _transcripts.Select(kv => kv.Value).Where(t => t is not SetupTranscript));

        var setupContext = turnEngine.CreateTurnContext(SystemPlayer.SystemPlayerId);
        foreach (var setupScript in _transcripts.Where(t => t.Value is SetupTranscript).OrderBy(kv => kv.Key).Select(kv => kv.Value))
        {
            setupScript.Execute(setupContext);
        }

        return turnEngine;
    }



    public TurnManagerBuilder AddPlayer(Player player)
    {
        _players.Add(player);

        return this;
    }

    public TurnManagerBuilder AddActor(Actor actor)
    {
        _actors.Add(actor);

        return this;
    }

    public TurnManagerBuilder AddTranscript(int group, TurnTranscript transcript)
    {
        var groupOffset = _transcripts.Count(kv => kv.Key >= group && kv.Key < group + 100);
        _transcripts.Add(group + groupOffset, transcript);

        return this;
    }
}
