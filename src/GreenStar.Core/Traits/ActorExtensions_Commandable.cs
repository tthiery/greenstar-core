using System;
using System.Collections.Generic;

using GreenStar.Traits;

namespace GreenStar;

public static partial class ActorExtensions
{
    public static IEnumerable<Command> GetCommands(this Actor self)
        => self.TryGetTrait<Commandable>(out var commandable) ? commandable.GetCommands() : [];
}