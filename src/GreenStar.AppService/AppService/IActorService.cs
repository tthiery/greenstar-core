using System.Diagnostics.CodeAnalysis;
using System.Linq;

using GreenStar.Cartography;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.AppService;

public interface IActorService
{
    IEnumerable<Actor> GetAllActors(Guid gameId, Guid playerId);
    IEnumerable<Actor> GetAllKnownActors(Guid gameId, Guid playerId);
    IEnumerable<Actor> GetKnownActorInRectangle(Guid gameId, Guid playerId, Coordinate topLeft, Coordinate bottomRight);
    bool TryGetActor(Guid gameId, Guid playerId, Guid actorId, [NotNullWhen(returnValue: true)] out Actor? actor);
}

public class ActorDomainService : IActorService
{
    public IEnumerable<Actor> GetAllActors(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            return turnManager.Actors.AsQueryable().Select(a => Materialize(a, playerId, turnManager));
        }
        else
        {
            return [];
        }
    }

    public IEnumerable<Actor> GetAllKnownActors(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var actorView = new PlayerActorView(turnManager.Actors, playerId);
            return actorView.AsQueryable().Select(a => Materialize(a, playerId, turnManager));
        }
        else
        {
            return [];
        }
    }

    public IEnumerable<Actor> GetKnownActorInRectangle(Guid gameId, Guid playerId, Coordinate topLeft, Coordinate bottomRight)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var actorView = new PlayerActorView(turnManager.Actors, playerId);

            var foundActors = actorView.GetActors<Actor, Locatable>(trait =>
            {
                var other = trait.GetPosition(actorView);

                return (other.X > topLeft.X && other.X < bottomRight.X && other.Y > topLeft.Y && other.Y < bottomRight.Y);
            }).Select(a => Materialize(a, playerId, turnManager)); ;

            return foundActors;
        }
        else
        {
            return [];
        }


    }

    public bool TryGetActor(Guid gameId, Guid playerId, Guid actorId, [NotNullWhen(returnValue: true)] out Actor? actor)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var actorView = new PlayerActorView(turnManager.Actors, playerId);
            actor = actorView.GetActor(actorId);

            return actor is not null;
        }
        else
        {
            actor = null;
            return false;
        }
    }

    private Actor Materialize(Actor a, Guid playerId, TurnManager turnManager)
    {
        foreach (var trait in a.Traits.OfType<IMaterialize>())
        {
            trait.Materialize(turnManager, playerId);
        }

        return a;
    }
}