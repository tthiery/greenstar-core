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
            if (!_vectorShipLocation.HasOwnPosition && !IsFull)
            {
                var locationActor = game.GetActor(_vectorShipLocation.HostLocationActorId);

                int maxFuel = 0;
                int fuel = Fuel;
                int newFuel = 0;

                switch (Self)
                {
                    case Tanker _:
                        maxFuel = Range * 10;
                        break;
                    default:
                        maxFuel = Range;
                        break;
                }

                if (locationActor is Planet planet)
                {
                    var playerIdOfPlanet = planet.Trait<Associatable>().PlayerId;

                    if (playerIdOfPlanet != Guid.Empty)
                    {
                        var playerOfPlanet = game.Players.FirstOrDefault(p => p.Id == playerIdOfPlanet);

                        // if a populated planet is below, the ships need some iteration till it is full again
                        if (playerOfPlanet != null && playerOfPlanet.IsFriendlyTo(_associatable.PlayerId))
                        {
                            int missingFuel = maxFuel - fuel;
                            int maxPerIteration = maxFuel;

                            newFuel = (missingFuel <= maxPerIteration) ? missingFuel : maxPerIteration;
                        }
                    }

                }

                // check for tanker to refill the rest.
                if (newFuel + fuel < maxFuel)
                {
                    var tanker = locationActor.Trait<Hospitality>().ActorIds
                        .Select(id => game.GetActor(id))
                        .Where(t => t != Self) // do not let tanker try to refill on themselves
                        .OfType<Tanker>()
                        .Where(t => t.Trait<Associatable>().PlayerId == _associatable.PlayerId)
                        .FirstOrDefault();

                    if (tanker != null)
                    {
                        int requiredRemainingFuel = maxFuel - (newFuel + fuel);

                        int tankerFuel = tanker.Trait<VectorFlightCapable>().Fuel;

                        int fuelFromTanker = (tankerFuel > requiredRemainingFuel) ? requiredRemainingFuel : tankerFuel;

                        tanker.Trait<VectorFlightCapable>().Fuel -= fuelFromTanker;

                        newFuel += fuelFromTanker;
                    }
                }

                // refill resource stock on ship
                if (newFuel > 0)
                {
                    this.Fuel += newFuel;
                }
            }
        }
    }
}