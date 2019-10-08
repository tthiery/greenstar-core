using System.Collections.Generic;

namespace GreenStar.Core
{
    public interface IActorBuilder
    {
        IEnumerable<Actor> Create();
    }
}