using System.Collections.Generic;

namespace GreenStar.Traits;

public interface ICommandFactory
{
    IEnumerable<Command> GetCommands();
}
