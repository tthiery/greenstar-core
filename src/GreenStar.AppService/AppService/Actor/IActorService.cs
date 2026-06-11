using System.Diagnostics.CodeAnalysis;
using System.Linq;

using GreenStar.Cartography;
using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.AppService.Actor;

public interface IActorService
{
    IEnumerable<GreenStar.Actor> GetAllActors(Guid gameId, Guid playerId);
    IEnumerable<GreenStar.Actor> GetAllKnownActors(Guid gameId, Guid playerId);
    IEnumerable<GreenStar.Actor> GetKnownActorInRectangle(Guid gameId, Guid playerId, Coordinate topLeft, Coordinate bottomRight);
    bool TryGetActor(Guid gameId, Guid playerId, Guid actorId, [NotNullWhen(returnValue: true)] out GreenStar.Actor? actor);
}

public class ActorDomainService : IActorService
{
    public IEnumerable<GreenStar.Actor> GetAllActors(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            return turnManager.Actors.AsQueryable().Select(a => Materialize(a, turnManager));
        }
        else
        {
            return [];
        }
    }

    public IEnumerable<GreenStar.Actor> GetAllKnownActors(Guid gameId, Guid playerId)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var actorView = new PlayerActorView(turnManager.Actors, playerId);
            return actorView.AsQueryable().Select(a => Materialize(a, turnManager));
        }
        else
        {
            return [];
        }
    }

    public IEnumerable<GreenStar.Actor> GetKnownActorInRectangle(Guid gameId, Guid playerId, Coordinate topLeft, Coordinate bottomRight)
    {
        if (GameHolder.Games.TryGetValue(gameId, out var turnManager))
        {
            var actorView = new PlayerActorView(turnManager.Actors, playerId);

            var foundActors = actorView.GetActors<GreenStar.Actor, Locatable>(trait =>
            {
                var other = trait.GetPosition(actorView);

                return (other.X > topLeft.X && other.X < bottomRight.X && other.Y > topLeft.Y && other.Y < bottomRight.Y);
            }).Select(a => Materialize(a, turnManager)); ;

            return foundActors;
        }
        else
        {
            return [];
        }


    }

    public bool TryGetActor(Guid gameId, Guid playerId, Guid actorId, [NotNullWhen(returnValue: true)] out GreenStar.Actor? actor)
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

    private GreenStar.Actor Materialize(GreenStar.Actor a, TurnManager turnManager)
    {
        foreach (var trait in a.Traits.OfType<IMaterialize>())
        {
            trait.Materialize(turnManager);
        }

        return a;
    }
}