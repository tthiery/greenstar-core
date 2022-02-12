using System;

using GreenStar.Persistence;
using GreenStar.Stellar;

namespace GreenStar.Traits;

public class ColonizationCapable : Trait
{
    private readonly Locatable _locatable;
    private readonly Associatable _associatable;

    public bool IsLoaded { get; set; } = true;

    public ColonizationCapable(Locatable locatable, Associatable associatable)
    {
        this._locatable = locatable ?? throw new System.ArgumentNullException(nameof(locatable));
        this._associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
    }

    public override void Load(IPersistenceReader reader)
    {
        IsLoaded = reader.Read<bool>(nameof(IsLoaded));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write(nameof(IsLoaded), IsLoaded);
    }

    public void AutoColonizeOrRecruit(Context context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!_locatable.HasOwnPosition &&
            _locatable.GetHostLocationActor(context) is Planet planet)
        {
            if (IsLoaded)
            {
                ColonizePlanet(context, planet, _associatable.PlayerId);
            }
            else
            {
                RecruitColonists(planet, _associatable.PlayerId);
            }

        }
    }

    private void ColonizePlanet(Context context, Planet planet, Guid playerId)
    {
        if (planet == null)
        {
            throw new ArgumentNullException(nameof(planet));
        }

        var association = planet.Trait<Associatable>() ?? throw new Exception("Invalid State");

        if (!association.IsOwnedByAnyPlayer())
        {
            var population = planet.Trait<Populatable>() ?? throw new Exception("Invalid State");

            population.Population = 10;
            association.PlayerId = playerId;

            context.PlayerContext.SendMessageToPlayer(playerId, context.TurnContext.Turn, text: $"You colonized {planet.Trait<Associatable>().Name}.");
        }
    }

    private void RecruitColonists(Planet planet, Guid playerId)
    {
        if (planet == null)
        {
            throw new ArgumentNullException(nameof(planet));
        }

        bool result = false;

        var association = planet.Trait<Associatable>() ?? throw new Exception("Invalid State");

        if (association.IsOwnedByPlayer(playerId))
        {
            var population = planet.Trait<Populatable>() ?? throw new Exception("Invalid State");

            result = (population.Population >= 10);
        }

        IsLoaded = result;
    }
}
