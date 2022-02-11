using System;

using GreenStar.Algorithms;
using GreenStar.Ships;
using GreenStar.Traits;
using GreenStar.TurnEngine;

using Microsoft.Extensions.Options;

namespace GreenStar.Transcripts;

public record OrderMoveCommand(string Id, string Title, Guid ActorId, CommandArgument[] Arguments)
    : Command(Id, Title, ActorId, Arguments);

public class OrderMoveCommandTranscript : TraitCommandTranscript<OrderMoveCommand, VectorShip, VectorFlightCapable>
{
    private readonly IOptions<ResearchOptions> _options;

    public OrderMoveCommandTranscript(IOptions<ResearchOptions> options, OrderMoveCommand command)
        : base(command)
    {
        _options = options;
    }

    public override void Execute(Context context, OrderMoveCommand command, VectorShip ship, VectorFlightCapable trait)
    {
        if (Guid.TryParse(command.Arguments[0].Value, out var toId))
        {
            var to = context.ActorContext.GetActor(toId)
                ?? throw new ArgumentException("fail to load target actor id", nameof(command));

            trait.StartFlight(context.ActorContext, to, _options.Value);
        }
        else
        {
            throw new ArgumentException("fail to parse target actor id", nameof(command));
        }
    }
}