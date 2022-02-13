using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Algorithms;
using GreenStar.Cartography;
using GreenStar.Persistence;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Transcripts;

namespace GreenStar.Traits;

public class VectorFlightCapable : Trait, ICommandFactory
{
    private readonly Locatable _vectorShipLocation;
    private readonly Capable _vectorShipCapabilities;
    private readonly Associatable _associatable;

    public int Fuel { get; set; }
    public Fuels FuelType { get; set; } = Fuels.Traditional;

    public Vector RelativeMovement { get; private set; } = new Vector(0, 0);
    public Guid SourceActorId { get; private set; } = Guid.Empty;
    public Guid TargetActorId { get; private set; } = Guid.Empty;

    public VectorFlightCapable(Locatable locatable, Capable capable, Associatable associatable)
    {
        this._vectorShipLocation = locatable ?? throw new ArgumentNullException(nameof(locatable));
        this._vectorShipCapabilities = capable ?? throw new ArgumentNullException(nameof(capable));
        this._associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
    }
    public override void Load(IPersistenceReader reader)
    {
        Fuel = reader.Read<int>(nameof(Fuel));
        FuelType = reader.Read<Fuels>(nameof(FuelType));
        SourceActorId = reader.Read<Guid>(nameof(SourceActorId));
        TargetActorId = reader.Read<Guid>(nameof(TargetActorId));

        RelativeMovement = new Vector(
            reader.Read<long>(nameof(RelativeMovement) + ":DeltaX"),
            reader.Read<long>(nameof(RelativeMovement) + ":DeltaY"));
    }

    public override void Persist(IPersistenceWriter writer)
    {
        writer.Write(nameof(Fuel), Fuel);
        writer.Write(nameof(FuelType), FuelType);
        writer.Write(nameof(SourceActorId), SourceActorId);
        writer.Write(nameof(TargetActorId), TargetActorId);
        writer.Write(nameof(RelativeMovement) + ":DeltaX", RelativeMovement.DeltaX);
        writer.Write(nameof(RelativeMovement) + ":DeltaY", RelativeMovement.DeltaY);
    }

    public IEnumerable<Command> GetCommands()
    {
        yield return new OrderMoveCommand($"cmd-move", "Order Ship to Move", this.Self.Id, new[] {
            new CommandArgument("to", CommandArgumentDataType.LocatableAndHospitableReference, string.Empty)
        });

        if (ActiveFlight)
        {
            yield return new CancelMoveCommand("cmd-cancel-move", "Stop Flight", this.Self.Id, Array.Empty<CommandArgument>());
        }
    }

    public int Range
        => _vectorShipCapabilities.Of(ShipCapabilities.Range);
    public int Speed
        => _vectorShipCapabilities.Of(ShipCapabilities.Speed);
    public bool IsFull
        => Fuel >= Range;

    public bool ActiveFlight
        => RelativeMovement.Length > 0;

    public bool StartFlight(IActorContext actorContext, Actor to, ResearchOptions options)
    {
        if (actorContext == null)
        {
            throw new ArgumentNullException(nameof(actorContext));
        }

        if (to == null)
        {
            throw new ArgumentNullException(nameof(to));
        }

        if (!ActiveFlight)
        {
            var from = actorContext.GetActor(_vectorShipLocation.HostLocationActorId) ?? throw new InvalidOperationException("host location not found");

            var locatable = to.Trait<Locatable>() ?? throw new InvalidOperationException("target must be locatable");
            var host = to.Trait<Hospitality>() ?? throw new InvalidOperationException("target must be host");

            // if source and target are the same
            if (from == to)
            {
                return false;
            }
            // if source and target are in the same position
            else if (from.Trait<Locatable>().GetPosition(actorContext) == locatable.GetPosition(actorContext))
            {
                //TODO: Switch from one hospitability into the other
                return false;
            }
            else
            {
                var currentIterationDistance = Math.Min(Fuel, Speed);

                if (currentIterationDistance > 0)
                {
                    from.Trait<Hospitality>().Leave(actorContext, Self);

                    RelativeMovement = CalculateCurrentRelativeVector(_vectorShipLocation.GetPosition(actorContext), locatable.GetPosition(actorContext), currentIterationDistance, options);
                    SourceActorId = from.Id;
                    TargetActorId = to.Id;
                }

                return true;
            }

            // TODO: eleminate exact location if source
        }
        else
        {
            return false;
        }
    }

    public void StopFlight(IActorContext actorContext, ITurnContext turnContext)
    {
        if (ActiveFlight)
        {
            var source = actorContext.GetActor(SourceActorId);

            // started and immediatelly stopped
            if (source is not null && source.Trait<Locatable>().GetPosition(actorContext) == _vectorShipLocation.GetPosition(actorContext))
            {
                source.Trait<Hospitality>().Enter(Self);
            }
            // stoped somewhere in the dark
            else
            {
                var exactPosition = new ExactLocation();
                exactPosition.Trait<Locatable>().SetPosition(_vectorShipLocation.GetPosition(actorContext));

                exactPosition.Trait<Hospitality>().Enter(Self);
                exactPosition.Trait<Discoverable>().AddDiscoverer(_associatable.PlayerId, DiscoveryLevel.PropertyAware, turnContext.Turn);

                actorContext.AddActor(exactPosition);
            }

            ResetToNoFlight();
        }
    }

    public void UpdatePosition(Context context, ResearchOptions options)
    {
        if (ActiveFlight)
        {
            var to = context.ActorContext.GetActor(TargetActorId); // TODO: null means target vanished?

            if (to is null)
            {
                StopFlight(context.ActorContext, context.TurnContext);
            }
            else
            {
                var source = _vectorShipLocation.GetPosition(context.ActorContext);
                var tartet = to.Trait<Locatable>().GetPosition(context.ActorContext);

                var currentIterationDistance = Math.Min(Fuel, Speed);

                DecreaseFuel(currentIterationDistance);

                if (!TestIfReachable(source, tartet, currentIterationDistance, options))
                {
                    var v = CalculateCurrentRelativeVector(source, tartet, currentIterationDistance, options);
                    RelativeMovement = v;

                    // update position
                    _vectorShipLocation.SetPosition(source + v);

                    if (Fuel == 0)
                    {
                        StopFlight(context.ActorContext, context.TurnContext);
                    }
                }
                else
                {
                    _vectorShipLocation.SetPosition(to.Trait<Locatable>().GetPosition(context.ActorContext));
                    to.Trait<Hospitality>().Enter(Self);

                    ResetToNoFlight();
                }
            }
        }
    }

    private void ResetToNoFlight()
    {
        RelativeMovement = new Vector(0, 0);
        TargetActorId = Guid.Empty;
    }

    private bool TestIfReachable(Coordinate source, Coordinate target, int distanceInSpeedUnits, ResearchOptions options)
    {
        var v = target - source;

        return v.Length <= ResearchAlgorithms.ConvertTechnologyLevelToStellarDistance(options, distanceInSpeedUnits);
    }

    private Vector CalculateCurrentRelativeVector(Coordinate source, Coordinate target, int currentRangeInNextTurn, ResearchOptions options)
    {
        var totalVector = target - source;

        double factor = (1.0 * ResearchAlgorithms.ConvertTechnologyLevelToStellarDistance(options, currentRangeInNextTurn)) / (1.0 * totalVector.Length);

        return new Vector(
            (int)(1.0 * totalVector.DeltaX * factor),
            (int)(1.0 * totalVector.DeltaY * factor));
    }

    /// <summary>
    /// Decrease the fuel of all ships in the flight
    /// </summary>
    private void DecreaseFuel(int distanceInSpeedUnits)
    {
        int fuel = Fuel;

        if (fuel <= distanceInSpeedUnits)
        {
            Fuel = 0;
        }
        else
        {
            Fuel = fuel - Speed;
        }
    }

    public void TryRefillFuel(Context context)
    {
        if (!_vectorShipLocation.HasOwnPosition && !IsFull)
        {
            var locationActor = context.ActorContext.GetActor(_vectorShipLocation.HostLocationActorId) ?? throw new InvalidOperationException("host location vanished?");

            int fuel = Fuel;
            int newFuel = 0;
            int maxFuel = DetermineMaxFuel(Self);

            int missingFuel = maxFuel - fuel;

            newFuel += TryGatherFuelFromPlanet(context.PlayerContext, context.TurnContext, locationActor, missingFuel);

            int requiredRemainingFuel = missingFuel - newFuel;

            if (FuelType == Fuels.Traditional && !(Self is Tanker) && requiredRemainingFuel > 0)
            {
                newFuel += TryGatherFuelFromTanker(context.ActorContext, locationActor, requiredRemainingFuel);
            }

            // refill resource stock on ship
            if (newFuel > 0)
            {
                this.Fuel += newFuel;
            }
        }
    }

    private int DetermineMaxFuel(Actor actor)
    {
        int result;

        switch (actor)
        {
            case Tanker _:
                result = Range * 10;
                break;
            default:
                result = Range;
                break;
        }

        return result;
    }

    private int TryGatherFuelFromTanker(IActorContext actorContext, Actor locationActor, int requiredRemainingFuel)
    {
        int result = 0;

        // check for tanker to refill the rest.                    
        var tanker = locationActor.Trait<Hospitality>()?.ActorIds
            .Select(id => actorContext.GetActor(id))
            .Where(t => t != Self) // do not let tanker try to refill on themselves
            .OfType<Tanker>()
            .Where(t => t.Trait<Associatable>()?.PlayerId == _associatable.PlayerId)
            .FirstOrDefault();

        if (tanker != null)
        {
            int tankerFuel = tanker.Trait<VectorFlightCapable>().Fuel;

            int fuelFromTanker = Math.Min(requiredRemainingFuel, tankerFuel);

            tanker.Trait<VectorFlightCapable>().Fuel -= fuelFromTanker;

            result += fuelFromTanker;
        }

        return result;
    }

    private int TryGatherFuelFromPlanet(IPlayerContext playerContext, ITurnContext turnContext, Actor locationActor, int missingFuel)
    {
        int result = 0;

        if (locationActor is Planet planet)
        {
            var playerIdOfPlanet = planet.Trait<Associatable>()?.PlayerId ?? throw new Exception("Invalid State");

            // if a is a traditional ship, the population might provide the fuel
            if (FuelType == Fuels.Traditional)
            {
                result = TryGatherTraditionalFuelFromPlanet(playerContext, missingFuel, playerIdOfPlanet);
            }
            // if it is a bioship, the population is the fuel.
            else if (FuelType == Fuels.Biomass)
            {
                result = TryGatherBiomassFuelFromPlanet(playerContext, turnContext, missingFuel, planet, playerIdOfPlanet);
            }
        }

        return result;
    }

    private static int TryGatherBiomassFuelFromPlanet(IPlayerContext playerContext, ITurnContext turnContext, int missingFuel, Planet planet, Guid playerIdOfPlanet)
    {
        int result;
        var populationPerRangeUnit = 1_000;

        var population = planet.Trait<Populatable>() ?? throw new Exception("Invalid State");

        var availableRangeOnPlanet = (int)(population.Population / populationPerRangeUnit);

        result = Math.Min(missingFuel, availableRangeOnPlanet);

        if (result > 0)
        {
            var populationDigested = result * populationPerRangeUnit;
            population.Population -= populationDigested;

            playerContext.SendMessageToPlayer(playerIdOfPlanet, turnContext.Turn, type: "Info", text: $"A bioship at {planet.Trait<Nameable>().Name} ate {populationDigested} people.");
        }

        return result;
    }

    private int TryGatherTraditionalFuelFromPlanet(IPlayerContext playerContext, int missingFuel, Guid playerIdOfPlanet)
    {
        int result = 0;

        if (playerIdOfPlanet != Guid.Empty)
        {
            var playerOfPlanet = playerContext.GetPlayer(playerIdOfPlanet);

            if (playerOfPlanet != null && playerOfPlanet.IsFriendlyTo(_associatable.PlayerId))
            {
                result = missingFuel;
            }
        }

        return result;
    }
}
