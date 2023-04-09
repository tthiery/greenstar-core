using System;

using GreenStar.Algorithms;
using GreenStar.Persistence;

namespace GreenStar.Traits;

public class Populatable : Trait
{
    private readonly Nameable _nameable;
    private readonly Associatable _associatable;

    /// <summary>
    /// Population living in/on the actor
    /// </summary>
    public long Population { get; set; }

    /// <summary>
    /// The gravity affecting the population.
    /// </summary>
    public double Gravity { get; set; }

    /// <summary>
    /// The temperature affecting the population.
    /// </summary>
    public double SurfaceTemperature { get; set; }

    /// <summary>
    /// The percentage of energy on that planet in mining
    /// </summary>
    public int MiningPercentage { get; set; }

    public Populatable(Nameable nameable, Associatable associatable)
    {
        _nameable = nameable ?? throw new ArgumentNullException(nameof(nameable));
        _associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
    }

    public override void Load(IPersistenceReader reader)
    {
        Population = reader.Read<long>(nameof(Population));
        Gravity = reader.Read<double>(nameof(Gravity));
        SurfaceTemperature = reader.Read<double>(nameof(SurfaceTemperature));
        MiningPercentage = reader.Read<int>(nameof(MiningPercentage));
    }
    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write(nameof(Population), Population);
        writer.Write(nameof(Gravity), Gravity);
        writer.Write(nameof(SurfaceTemperature), SurfaceTemperature);
        writer.Write(nameof(MiningPercentage), MiningPercentage);
    }

    public void Life(Context context, PlanetLifeOptions planetLifeOptions)
    {
        if (_associatable.IsOwnedByAnyPlayer())
        {
            Terraform(context);

            GrowPopulation(context, planetLifeOptions);
        }
    }

    /// <summary>
    /// Adjust the temperature as a respose on enabled terraforming
    /// </summary>
    private void Terraform(Context context)
    {
        if (Population > 0)
        {
            var (_, idealTemperature) = RetrieveIdealConditionForPlayer(context.PlayerContext, _associatable.PlayerId);

            if (SurfaceTemperature != idealTemperature)
            {
                double newTemperature = PlanetAlgorithms.CalculateNewTemperature(SurfaceTemperature, idealTemperature);

                SurfaceTemperature = newTemperature;

                if (SurfaceTemperature == idealTemperature)
                {
                    context.PlayerContext.SendMessageToPlayer(_associatable.PlayerId, context.TurnContext.Turn,
                        type: "Info",
                        text: $"You have finished the terraforming of {_nameable.Name}"
                    );
                }
            }
        }
    }

    private void GrowPopulation(Context context, PlanetLifeOptions planetLifeOptions)
    {
        long population = Population;

        if (population > 0)
        {
            var (idealGravity, idealTemperature) = RetrieveIdealConditionForPlayer(context.PlayerContext, _associatable.PlayerId);

            var newPopulation = PlanetAlgorithms.CalculateNewPopulation(
                planetLifeOptions,
                population,
                Gravity, SurfaceTemperature,
                idealGravity, idealTemperature,
                100);

            Population = newPopulation;
        }
    }

    private (double idealGravity, double idealTemperature) RetrieveIdealConditionForPlayer(IPlayerContext playerContext, Guid playerId)
    {
        var player = playerContext.GetPlayer(playerId);

        return (player?.IdealConditions?.IdealGravity ?? 1, player?.IdealConditions?.IdealTemperature ?? 20);
    }
}
