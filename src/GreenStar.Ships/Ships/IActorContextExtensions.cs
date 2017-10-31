using System;
using GreenStar.Core;
using GreenStar.Core.Traits;
using GreenStar.Core.TurnEngine;

namespace GreenStar.Ships
{
    public static class IActorContextExtensions
    {
        public static Ship AddShip<T>(this IActorContext self, Guid playerId, int range = 0, int speed = 0, int attack = 0, int defense = 0, int mini = 0) where T: Ship, new()
        {
            var ship = new T();

            ship.Trait<Associatable>().PlayerId = playerId;
            ship.Trait<Capable>().Of(ShipCapabilities.Range, range);
            ship.Trait<Capable>().Of(ShipCapabilities.Speed, speed);
            ship.Trait<Capable>().Of(ShipCapabilities.Attack, attack);
            ship.Trait<Capable>().Of(ShipCapabilities.Defense, defense);
            ship.Trait<Capable>().Of(ShipCapabilities.Mini, mini);
            self.AddActor(ship);

            return ship;
        }
    }
}