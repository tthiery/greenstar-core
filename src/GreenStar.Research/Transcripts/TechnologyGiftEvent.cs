using System;
using System.Linq;
using System.Threading.Tasks;

using GreenStar;
using GreenStar.Research;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class TechnologyGiftEvent : EventTranscript
{
    private readonly IPlayerTechnologyStateLoader _loader;
    private readonly TechnologyProgressEngine _technologyManager;
    private readonly string _text;
    private readonly string[] _arguments;

    public TechnologyGiftEvent(IPlayerTechnologyStateLoader loader, TechnologyProgressEngine technologyManager, string text, string[] arguments)
    {
        _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        _technologyManager = technologyManager ?? throw new ArgumentNullException(nameof(technologyManager));
        _text = text;
        _arguments = arguments;
    }

    public override async Task ExecuteAsync(Context context)
    {
        if (context.Player is null)
        {
            throw new InvalidOperationException("the event can only be executed in context of a player.");
        }

        if (_arguments.Length != 2)
        {
            throw new InvalidOperationException("the event has to be parameterized by two values");
        }

        var technologyName = _arguments[0];
        var change = Convert.ToInt32(_arguments[1]);

        var state = await _loader.LoadAsync(context.Player.Id);

        (state, var levelUp) = _technologyManager.IncreaseLevel(state, technologyName, change);

        if (levelUp is not null)
        {
            await _loader.SaveAsync(context.Player.Id, state);

            await ResearchTurnTranscript.ExecuteLevelUpAsync(context, context.Player, levelUp);
        }
    }
}