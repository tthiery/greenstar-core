using System;

using GreenStar.TurnEngine;

namespace GreenStar.Traits;

// is invoked before a object is hydrated to an API or similiar to make sure all reported properties are materialized and up-2-date
public interface IMaterialize
{
    //TODO: Materialize for the one player (fake it if needed)
    void Materialize(TurnManager turnManager, Guid playerId);
}