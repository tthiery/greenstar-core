using System.Collections.Generic;

namespace GreenStar;

public interface IActorBuilder
{
    IEnumerable<Actor> Create();
}
