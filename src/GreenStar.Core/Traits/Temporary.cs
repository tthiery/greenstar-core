using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Persistence;
using GreenStar.Transcripts;

namespace GreenStar.Traits;

public interface IAliveCheck
{
    bool IsAlive { get; }
}

public class Temporary : Trait, ICommandFactory
{
    public bool IsPermanent { get; set; } = false;
    public bool IsPermanentOverride { get; set; } = false;

    public bool IsInUseByTraits
        => Self.Traits.OfType<IAliveCheck>().Any(t => t.IsAlive);

    public bool IsAlive => IsPermanent || IsPermanentOverride || IsInUseByTraits;

    public IEnumerable<Command> GetCommands()
    {
        if (IsPermanent == false)
        {
            if (IsPermanentOverride == false)
            {
                yield return new MakePermanentCommand("cmd-make-permanent", "Make Permanent", this.Self.Id, Array.Empty<CommandArgument>());
            }
            else if (IsInUseByTraits == false)
            {
                yield return new DeleteTemporaryCommand("cmd-delete-temporary", "Remove", this.Self.Id, Array.Empty<CommandArgument>());
            }
        }
    }

    public override void Load(IPersistenceReader reader)
    {
        IsPermanentOverride = reader.Read<bool>(nameof(IsPermanentOverride));
        IsPermanent = reader.Read<bool>(nameof(IsPermanent));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write<bool>(nameof(IsPermanentOverride), IsPermanentOverride);
        writer.Write<bool>(nameof(IsPermanent), IsPermanent);
    }
}