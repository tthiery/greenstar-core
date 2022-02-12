using System;
using System.Threading.Tasks;

using GreenStar;
using GreenStar.Resources;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class ResourceGiftEvent : EventTranscript
{
    private readonly string _text;
    private readonly string[] _arguments;

    public ResourceGiftEvent(string text, string[] arguments)
    {
        _text = text;
        _arguments = arguments;
    }

    public override Task ExecuteAsync(Context context)
    {
        if (context.Player is null)
        {
            throw new InvalidOperationException("event can only be fired in context of a player");
        }

        ResourceAmount amount = _arguments[0];

        //TODO: move this to Invoicing
        context.Player.Resources += amount;

        context.PlayerContext.SendMessageToPlayer(context.Player.Id, context.TurnContext.Turn, text: _text);

        return Task.CompletedTask;
    }
}