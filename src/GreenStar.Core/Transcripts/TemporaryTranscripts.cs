using System;
using System.Threading.Tasks;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public record MakePermanentCommand(string Id, string Title, Guid ActorId, CommandArgument[] Arguments)
    : Command(Id, Title, ActorId, Arguments);
public record DeleteTemporaryCommand(string Id, string Title, Guid ActorId, CommandArgument[] Arguments)
    : Command(Id, Title, ActorId, Arguments);


public class MakePermanentCommandTranscript : TraitCommandTranscript<MakePermanentCommand, Actor, Temporary>
{
    public MakePermanentCommandTranscript(MakePermanentCommand command)
        : base(command)
    { }

    public override Task ExecuteAsync(Context context, MakePermanentCommand command, Actor actor, Temporary trait)
    {
        trait.IsPermanentOverride = true;

        return Task.CompletedTask;
    }
}

public class DeleteTemporaryCommandTranscript : TraitCommandTranscript<DeleteTemporaryCommand, Actor, Temporary>
{
    public DeleteTemporaryCommandTranscript(DeleteTemporaryCommand command)
        : base(command)
    { }

    public override Task ExecuteAsync(Context context, DeleteTemporaryCommand command, Actor actor, Temporary trait)
    {
        trait.IsPermanentOverride = false;

        return Task.CompletedTask;
    }
}

public class TemporaryTranscript : TraitTurnTranscript<Actor, Temporary>
{
    public override Task ExecuteTraitAsync(Context context, Actor actor, Temporary trait)
    {
        if (trait.IsAlive == false)
        {
            context.ActorContext.RemoveActor(actor);
        }

        return Task.CompletedTask;
    }
}