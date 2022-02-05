using System;
using System.Collections.Generic;

using GreenStar.Traits;

namespace GreenStar;

public static partial class ActorExtensions
{
    public static IEnumerable<Command> GetCommands(this Actor self)
        => self.Trait<Commandable>()?.GetCommands() ?? Array.Empty<Command>();
}