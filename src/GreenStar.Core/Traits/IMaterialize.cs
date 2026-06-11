using GreenStar.TurnEngine;

namespace GreenStar.Traits;

// is invoked before a object is hydrated to an API or similiar to make sure all reported properties are materialized and up-2-date
public interface IMaterialize
{
    void Materialize(TurnManager turnManager);
}