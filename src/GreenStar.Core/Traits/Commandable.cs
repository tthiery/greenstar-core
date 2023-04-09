using System.Collections.Generic;
using System.Linq;

namespace GreenStar.Traits;

public class Commandable : Trait
{
    public IEnumerable<Command> GetCommands()
    {
        var commandFactories = Self.Traits
            .OfType<ICommandFactory>();
        // could union here with other sourcs of command factories

        return commandFactories.SelectMany(cmdFactory => cmdFactory.GetCommands());
    }
}
