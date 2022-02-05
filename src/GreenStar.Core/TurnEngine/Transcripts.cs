using System;
using System.Collections.Generic;

namespace GreenStar.TurnEngine;

public abstract class Transcript
{
    public abstract void Execute(Context context);
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
    public override void Execute(Context context)
    {
        if (context.Player is null)
        {
            throw new ArgumentException("player property is not set", nameof(context));
        }
        Execute(context, _command);
    }

    public abstract void Execute(Context context, TCommand command);
}

public abstract class TraitCommandTranscript<TCommand, TActor, TTrait> : CommandTranscript<TCommand>
    where TCommand : Command
    where TActor : Actor
    where TTrait : Trait
{
    protected TraitCommandTranscript(TCommand command)
        : base(command)
    { }

    public abstract void Execute(Context context, TCommand command, TActor actor, TTrait trait);

    public override void Execute(Context context, TCommand command)
    {
        var actor = context.ActorContext.GetActor<TActor>(command.ActorId)
            ?? throw new ArgumentException("invalid ship actor id provided", nameof(command));

        var trait = actor.Trait<TTrait>();

        Execute(context, command, actor, trait);
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
    public override void Execute(Context context)
    {
        foreach (var actor in context.ActorContext.GetActors<TActor, TTrait>())
        {
            var trait = actor.Trait<TTrait>();

            ExecuteTrait(context, actor, trait);
        }
    }

    public abstract void ExecuteTrait(Context context, Actor actor, TTrait trait);
}