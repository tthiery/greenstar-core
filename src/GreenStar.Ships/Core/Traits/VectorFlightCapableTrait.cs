using System;
using GreenStar.Algorithms;
using GreenStar.Core.Cartography;
using GreenStar.Core.Persistence;
using GreenStar.Core.TurnEngine;
using GreenStar.Ships;
using GreenStar.Stellar;

namespace GreenStar.Core.Traits
{
    public class VectorFlightCapableTrait : Trait
    {
        public VectorFlightCapableTrait()
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

        public int Speed { get; private set; }
        public int Fuel { get; set; }

        public Vector RelativeMovement { get; private set; }
        public Guid TargetActorId { get; private set; }

        public bool ActiveFlight
            => Speed > 0;

        public void StartFlight(Game game, Actor to)
        {
            if (!ActiveFlight)
            {
                var from = game.GetActor(Self.Trait<LocatableTrait>().HostLocationActorId);
                
                var locatable = to.Trait<LocatableTrait>() ?? throw new InvalidOperationException("target must be locatable");
                var host = to.Trait<HostTrait>() ?? throw new InvalidOperationException("target must be host");

                from.Trait<HostTrait>().Leave(Self);

                Speed = 5; // TODO
                TargetActorId = to.Id;
            }
        }

        public void StopFlight(Game game)
        {
            if (ActiveFlight)
            {
                var exactPosition = new ExactLocation(Guid.NewGuid());
                exactPosition.Trait<LocatableTrait>().Position = Self.Trait<LocatableTrait>().Position;

                exactPosition.Trait<HostTrait>().Enter(Self);
                exactPosition.Trait<DiscoverableTrait>().AddDiscoverer(Self.Trait<AssociatableTrait>().PlayerId, DiscoveryLevel.PropertyAware, game.Turn);

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

                var source = Self.Trait<LocatableTrait>().Position;
                var tartet = to.Trait<LocatableTrait>().Position;

                var distanceInSpeedUnits = Math.Min(Fuel, Speed);

                DecreaseFuel(distanceInSpeedUnits);

                if (!TestIfReachable(source, tartet, distanceInSpeedUnits))
                {
                    var v = CalculateCurrentRelativeVector(source, tartet, distanceInSpeedUnits);
                    RelativeMovement = v;

                    Self.Trait<LocatableTrait>().Position = source + v;

                    if (Fuel == 0)
                    {
                        StopFlight(game);
                    }
                }
                else
                {
                    Self.Trait<LocatableTrait>().Position = to.Trait<LocatableTrait>().Position;
                    to.Trait<HostTrait>().Enter(Self);

                    ResetToNoFlight();
                }
            }
        }

        private void ResetToNoFlight()
        {
            RelativeMovement = new Vector(0, 0);
            TargetActorId = Guid.Empty;
            Speed = 0;
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