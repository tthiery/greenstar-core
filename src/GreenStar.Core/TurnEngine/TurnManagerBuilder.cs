using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

namespace GreenStar.TurnEngine;

public class TurnManagerBuilder
{
    private readonly List<Player> _players;
    private readonly List<Actor> _actors;
    private readonly SortedList<int, TurnTranscript> _transcripts;
    public IServiceProvider ServiceProvider { get; }

    public TurnManagerBuilder(IServiceProvider serviceProvider)
    {
        _players = new List<Player>();
        _actors = new List<Actor>();
        _transcripts = new SortedList<int, TurnTranscript>();
        ServiceProvider = serviceProvider;
    }

    public async Task<TurnManager> BuildAsync()
    {
        var actorStore = new InMemoryActorStore(Guid.NewGuid(), _actors);

        var turnEngine = new TurnManager(ServiceProvider, actorStore, new InMemoryPlayerStore(_players), _transcripts.Select(kv => kv.Value).Where(t => t is not SetupTranscript));

        var setupContext = turnEngine.CreateTurnContext(SystemPlayer.SystemPlayerId);
        foreach (var setupScript in _transcripts.Where(t => t.Value is SetupTranscript).OrderBy(kv => kv.Key).Select(kv => kv.Value))
        {
            await setupScript.ExecuteAsync(setupContext);
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

    public TurnManagerBuilder AddTranscript<TTranscript>(int group)
        where TTranscript : TurnTranscript
    {
        var transcript = ActivatorUtilities.CreateInstance<TTranscript>(ServiceProvider) as TTranscript;

        return AddTranscript(group, transcript);
    }
}
