using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenStar.TurnEngine;

public abstract class Transcript
{
    public abstract Task ExecuteAsync(Context context);
}

public abstract class EventTranscript : Transcript
{ }

public abstract class CommandTranscript<TCommand> : Transcript
    where TCommand : Command
{
    public CommandTranscript(TCommand command)
    {
        _command = command;
    }
    private TCommand _command { get; }
    public override async Task ExecuteAsync(Context context)
    {
        if (context.Player is null)
        {
            throw new ArgumentException("player property is not set", nameof(context));
        }
        await ExecuteAsync(context, _command);
    }

    public abstract Task ExecuteAsync(Context context, TCommand command);
}

public abstract class TraitCommandTranscript<TCommand, TActor, TTrait> : CommandTranscript<TCommand>
    where TCommand : Command
    where TActor : Actor
    where TTrait : Trait
{
    protected TraitCommandTranscript(TCommand command)
        : base(command)
    { }

    public abstract Task ExecuteAsync(Context context, TCommand command, TActor actor, TTrait trait);

    public override async Task ExecuteAsync(Context context, TCommand command)
    {
        var actor = context.ActorContext.GetActor<TActor>(command.ActorId)
            ?? throw new ArgumentException("invalid ship actor id provided", nameof(command));

        var trait = actor.Trait<TTrait>();

        await ExecuteAsync(context, command, actor, trait);
    }
}

public abstract class TurnTranscript : Transcript
{
    public Dictionary<string, object> IntermediateData { get; set; } = new Dictionary<string, object>();
}

public abstract class SetupTranscript : TurnTranscript
{ }

public abstract class TraitTurnTranscript<TActor, TTrait> : TurnTranscript
    where TActor : Actor
    where TTrait : Trait
{
    public override async Task ExecuteAsync(Context context)
    {
        foreach (var actor in context.ActorContext.GetActors<TActor, TTrait>())
        {
            var trait = actor.Trait<TTrait>();

            await ExecuteTraitAsync(context, actor, trait);
        }
    }

    public abstract Task ExecuteTraitAsync(Context context, Actor actor, TTrait trait);
}