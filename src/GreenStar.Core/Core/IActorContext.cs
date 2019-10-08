using System;
using System.Linq;

namespace GreenStar.Core
{
    public interface IActorContext
    {
        void AddActor(Actor actor);
        void RemoveActor(Actor actor);
        Actor GetActor(Guid actorId);
        IQueryable<Actor> AsQueryable();
    }
}