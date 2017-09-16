using System;
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

        public VectorFlightCapable(Locatable locatable, Capable capable)
        {
            if (locatable == null)
            {
                throw new ArgumentNullException(nameof(locatable));
            }

            if (capable == null)
            {
                throw new ArgumentNullException(nameof(capable));
            }

            this._vectorShipLocation = locatable;
            this._vectorShipCapabilities = capable;
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

        public int Speed
            => _vectorShipCapabilities.Of(ShipCapabilities.Speed);

        public int Fuel { get; set; }

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
                var exactPosition = new ExactLocation(Guid.NewGuid());
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
    }
}