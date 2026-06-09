using System;
using System.Linq;
using System.Collections.Generic;

using GreenStar.Traits;

namespace GreenStar;

public static partial class ActorExtensions
{
    public static IEnumerable<Command> GetCommands(this Actor self)
        => self.Traits
            .OfType<ICommandFactory>()
            .SelectMany(cmdFactory => cmdFactory.GetCommands());
}