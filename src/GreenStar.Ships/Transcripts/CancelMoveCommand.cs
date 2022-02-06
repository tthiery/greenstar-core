using System;

using GreenStar.Ships;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public record CancelMoveCommand(string Id, string Title, Guid ActorId, CommandArgument[] Arguments)
    : Command(Id, Title, ActorId, Arguments);

public class CancelMoveCommandTranscript : TraitCommandTranscript<CancelMoveCommand, VectorShip, VectorFlightCapable>
{
    public CancelMoveCommandTranscript(CancelMoveCommand command)
        : base(command)
    { }

    public override void Execute(Context context, CancelMoveCommand command, VectorShip ship, VectorFlightCapable trait)
    {
        trait.StopFlight(context.ActorContext, context.TurnContext);
    }
}