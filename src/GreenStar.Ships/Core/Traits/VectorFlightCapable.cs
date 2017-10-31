using System;
using System.Linq;
using GreenStar.Algorithms;
using GreenStar.Core.Cartography;
using GreenStar.Core.Persistence;
using GreenStar.Core.TurnEngine;
using GreenStar.Ships;
using GreenStar.Stellar;

namespace GreenStar.Core.Traits
{
    public class VectorFlightCapable : Trait
    {
        private readonly Locatable _vectorShipLocation;
        private readonly Capable _vectorShipCapabilities;
        private readonly Associatable _associatable;

        public VectorFlightCapable(Locatable locatable, Capable capable, Associatable associatable)
        {
            this._vectorShipLocation = locatable ?? throw new ArgumentNullException(nameof(locatable));
            this._vectorShipCapabilities = capable ?? throw new ArgumentNullException(nameof(capable));
            this._associatable = associatable ?? throw new ArgumentNullException(nameof(associatable));
        }
        public VectorFlightCapable()
        {
            ResetToNoFlight();
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

        public void StartFlight(Game game, Actor to)
        {
            if (!ActiveFlight)
            {
                var from = game.GetActor(Self.Trait<Locatable>().HostLocationActorId);

                var locatable = to.Trait<Locatable>() ?? throw new InvalidOperationException("target must be locatable");
                var host = to.Trait<Hospitality>() ?? throw new InvalidOperationException("target must be host");

                var distanceInSpeedUnits = Math.Min(Fuel, Speed);

                if (distanceInSpeedUnits > 0)
                {
                    from.Trait<Hospitality>().Leave(Self);

                    RelativeMovement = CalculateCurrentRelativeVector(_vectorShipLocation.Position, locatable.Position, distanceInSpeedUnits);
                    TargetActorId = to.Id;
                }
            }
        }

        public void StopFlight(Game game)
        {
            if (ActiveFlight)
            {
                var exactPosition = new ExactLocation();
                exactPosition.Trait<Locatable>().Position = Self.Trait<Locatable>().Position;

                exactPosition.Trait<Hospitality>().Enter(Self);
                exactPosition.Trait<Discoverable>().AddDiscoverer(Self.Trait<Associatable>().PlayerId, DiscoveryLevel.PropertyAware, game.Turn);

                game.AddActor(exactPosition);

                ResetToNoFlight();
            }
        }

        public void UpdatePosition(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (ActiveFlight)
            {
                var to = game.GetActor(TargetActorId);

                var source = _vectorShipLocation.Position;
                var tartet = to.Trait<Locatable>().Position;

                var distanceInSpeedUnits = Math.Min(Fuel, Speed);

                DecreaseFuel(distanceInSpeedUnits);

                if (!TestIfReachable(source, tartet, distanceInSpeedUnits))
                {
                    var v = CalculateCurrentRelativeVector(source, tartet, distanceInSpeedUnits);
                    RelativeMovement = v;

                    _vectorShipLocation.Position = source + v;

                    if (Fuel == 0)
                    {
                        StopFlight(game);
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
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

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

        public void TryRefillFuel(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (!_vectorShipLocation.HasOwnPosition && !IsFull)
            {
                var locationActor = game.GetActor(_vectorShipLocation.HostLocationActorId);

                int fuel = Fuel;
                int newFuel = 0;
                int maxFuel = DetermineMaxFuel(Self);

                int missingFuel = maxFuel - fuel;

                newFuel += TryGatherFuelFromPlanet(game, locationActor, missingFuel);

                int requiredRemainingFuel = missingFuel - newFuel;

                if (FuelType == Fuels.Traditional && !(Self is Tanker) && requiredRemainingFuel > 0)
                {
                    newFuel += TryGatherFuelFromTanker(game, locationActor, requiredRemainingFuel);
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

        private int TryGatherFuelFromTanker(Game game, Actor locationActor, int requiredRemainingFuel)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (locationActor == null)
            {
                throw new ArgumentNullException(nameof(locationActor));
            }

            int result = 0;

            // check for tanker to refill the rest.                    
            var tanker = locationActor.Trait<Hospitality>()?.ActorIds
                .Select(id => game.GetActor(id))
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

        private int TryGatherFuelFromPlanet(Game game, Actor locationActor, int missingFuel)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            int result = 0;

            if (locationActor is Planet planet)
            {
                var playerIdOfPlanet = planet.Trait<Associatable>()?.PlayerId ?? throw new Exception("Invalid State");

                // if a is a traditional ship, the population might provide the fuel
                if (FuelType == Fuels.Traditional)
                {
                    result = TryGatherTraditionalFuelFromPlanet(game, missingFuel, playerIdOfPlanet);
                }
                // if it is a bioship, the population is the fuel.
                else if (FuelType == Fuels.Biomass)
                {
                    result = TryGatherBiomassFuelFromPlanet(game, missingFuel, planet, playerIdOfPlanet);
                }
            }

            return result;
        }

        private static int TryGatherBiomassFuelFromPlanet(Game game, int missingFuel, Planet planet, Guid playerIdOfPlanet)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
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

                game.SendMessage(playerIdOfPlanet, type: "Info", text: $"A bioship at {planet.Trait<Associatable>().Name} ate {populationDigested} people.");
            }

            return result;
        }

        private int TryGatherTraditionalFuelFromPlanet(Game game, int missingFuel, Guid playerIdOfPlanet)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            int result = 0;

            if (playerIdOfPlanet != Guid.Empty)
            {
                var playerOfPlanet = game.Players.FirstOrDefault(p => p.Id == playerIdOfPlanet);

                if (playerOfPlanet != null && playerOfPlanet.IsFriendlyTo(_associatable.PlayerId))
                {
                    result = missingFuel;
                }
            }

            return result;
        }
    }
}