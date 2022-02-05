using System;

using GreenStar.Ships;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public record OrderMoveCommand(string Id, string Title, Guid ActorId, CommandArgument[] Arguments)
    : Command(Id, Title, ActorId, Arguments);

public class OrderMoveCommandTranscript : TraitCommandTranscript<OrderMoveCommand, VectorShip, VectorFlightCapable>
{
    public OrderMoveCommandTranscript(OrderMoveCommand command)
        : base(command)
    { }

    public override void Execute(Context context, OrderMoveCommand command, VectorShip ship, VectorFlightCapable trait)
    {
        if (Guid.TryParse(command.Arguments[0].Value, out var toId))
        {
            var to = context.ActorContext.GetActor(toId)
                ?? throw new ArgumentException("fail to load target actor id", nameof(command));

            trait.StartFlight(context.ActorContext, to);
        }
        else
        {
            throw new ArgumentException("fail to parse target actor id", nameof(command));
        }
    }
}