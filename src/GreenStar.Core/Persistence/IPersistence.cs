using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GreenStar.TurnEngine;

namespace GreenStar.Persistence;

public interface IPersistence
{
    Task PersistFullAsync(ITurnContext turnContext, IPlayerContext playerContext, IActorContext actorContext);

    Task<(int, IEnumerable<Player>, IEnumerable<Actor>)> LoadFullAsync();
}
