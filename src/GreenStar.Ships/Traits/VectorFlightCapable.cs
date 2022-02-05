using System;
using System.Linq;

using GreenStar.Algorithms;
using GreenStar.Cartography;
using GreenStar.Persistence;
using GreenStar.Ships;
using GreenStar.Stellar;
using System.Collections.Generic;
using GreenStar.Transcripts;

namespace GreenStar.Traits;

public class VectorFlightCapable : Trait, ICommandFactory
{
    public IEnumerable<Command> GetCommands()
    {
        yield return new OrderMoveCommand($"cmd-move", "Order Ship to Move", this.Self.Id, Array.Empty<string>());
    }

    private readonly Locatable _vectorShipLocation;
    private readonly Capable _vectorShipCapabilities;
    private readonly Associatable _associatable;

    public VectorFlightCapable(Locatable locatable, Capable capable, Associatable associatable)
    {
        this._vectorShipLocation = locatable ?? throw new ArgumentNullException(nameof(locatable));
        this._vectorShipCapabilities = capable ?? throw new ArgumentNullException(nameof(capable));
        this._associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
    }
    public override void Load(IPersistenceReader reader)
    {
        throw new System.NotImplementedException();
    }

    public override void Persist(IPersistenceWriter writer)
    {
        throw new System.NotImplementedException();
    }

    public int Fuel { get; set; }

    public Fuels FuelType { get; set; } = Fuels.Traditional;

    public int Range
        => _vectorShipCapabilities.Of(ShipCapabilities.Range);
    public int Speed
        => _vectorShipCapabilities.Of(ShipCapabilities.Speed);

    public bool IsFull
        => Fuel >= Range;

    public Vector RelativeMovement { get; private set; } = new Vector(0, 0);
    public Guid TargetActorId { get; private set; } = Guid.Empty;

    public bool ActiveFlight
        => RelativeMovement.Length > 0;

    public void StartFlight(IActorContext actorContext, Actor to)
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

            var distanceInSpeedUnits = Math.Min(Fuel, Speed);

            if (distanceInSpeedUnits > 0)
            {
                from.Trait<Hospitality>().Leave(Self);

                RelativeMovement = CalculateCurrentRelativeVector(_vectorShipLocation.Position, locatable.Position, distanceInSpeedUnits);
                TargetActorId = to.Id;
            }

            // TODO: eleminate exact location if soure
        }
    }

    public void StopFlight(IActorContext actorContext, ITurnContext turnContext)
    {
        if (ActiveFlight)
        {
            var exactPosition = new ExactLocation();
            exactPosition.Trait<Locatable>().Position = _vectorShipLocation.Position;

            exactPosition.Trait<Hospitality>().Enter(Self);
            exactPosition.Trait<Discoverable>().AddDiscoverer(_associatable.PlayerId, DiscoveryLevel.PropertyAware, turnContext.Turn);

            actorContext.AddActor(exactPosition);

            ResetToNoFlight();
        }
    }

    public void UpdatePosition(Context context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (ActiveFlight)
        {
            var to = context.ActorContext.GetActor(TargetActorId); // TODO: null means target vanished?

            var source = _vectorShipLocation.Position;
            var tartet = to.Trait<Locatable>().Position;

            var currentIterationDistance = Math.Min(Fuel, Speed);

            DecreaseFuel(currentIterationDistance);

            if (!TestIfReachable(source, tartet, currentIterationDistance))
            {
                var v = CalculateCurrentRelativeVector(source, tartet, currentIterationDistance);
                RelativeMovement = v;

                _vectorShipLocation.Position = source + v;

                if (Fuel == 0)
                {
                    StopFlight(context.ActorContext, context.TurnContext);
                }
            }
            else
            {
                _vectorShipLocation.Position = to.Trait<Locatable>().Position;
                to.Trait<Hospitality>().Enter(Self);

                ResetToNoFlight();
            }
        }
    }

    private void ResetToNoFlight()
    {
        RelativeMovement = new Vector(0, 0);
        TargetActorId = Guid.Empty;
    }

    private bool TestIfReachable(Coordinate source, Coordinate target, int distanceInSpeedUnits)
    {
        var v = target - source;

        return v.Length <= ResearchAlgorithms.ConvertTechnologyLevelToStellarDistance(distanceInSpeedUnits);
    }

    private Vector CalculateCurrentRelativeVector(Coordinate source, Coordinate target, int distanceInSpeedUnits)
    {
        var totalVector = target - source;

        double factor = (1.0 * ResearchAlgorithms.ConvertTechnologyLevelToStellarDistance(distanceInSpeedUnits)) / (1.0 * totalVector.Length);

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
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

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
        if (actor == null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

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
        if (actorContext == null)
        {
            throw new ArgumentNullException(nameof(actorContext));
        }

        if (locationActor == null)
        {
            throw new ArgumentNullException(nameof(locationActor));
        }

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
        if (playerContext == null)
        {
            throw new ArgumentNullException(nameof(playerContext));
        }

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
        if (playerContext == null)
        {
            throw new ArgumentNullException(nameof(playerContext));
        }

        if (planet == null)
        {
            throw new ArgumentNullException(nameof(planet));
        }

        int result;
        var populationPerRangeUnit = 1_000;

        var population = planet.Trait<Populatable>() ?? throw new Exception("Invalid State");

        var availableRangeOnPlanet = (int)(population.Population / populationPerRangeUnit);

        result = Math.Min(missingFuel, availableRangeOnPlanet);

        if (result > 0)
        {
            var populationDigested = result * populationPerRangeUnit;
            population.Population -= populationDigested;

            playerContext.SendMessageToPlayer(playerIdOfPlanet, turnContext.Turn, type: "Info", text: $"A bioship at {planet.Trait<Associatable>().Name} ate {populationDigested} people.");
        }

        return result;
    }

    private int TryGatherTraditionalFuelFromPlanet(IPlayerContext playerContext, int missingFuel, Guid playerIdOfPlanet)
    {
        if (playerContext == null)
        {
            throw new ArgumentNullException(nameof(playerContext));
        }

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
