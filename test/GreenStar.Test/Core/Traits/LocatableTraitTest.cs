using System;
using GreenStar.Core.Cartography;
using GreenStar.Core.TurnEngine;
using GreenStar.Ships;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class LocatableTest
    {
        [Fact]
        public void Locatable_Leave_Simple()
        {
            // arrange
            var exactLocation = new ExactLocation();
            exactLocation.Trait<Locatable>().Position = new Coordinate(10, 10);

            var scout = new Scout();
            exactLocation.Trait<Hospitality>().Enter(scout);

            var game = new Game(Guid.NewGuid(), new Player[0], new Actor[] { exactLocation, scout });

            // act
            var coordinate = exactLocation.Trait<Locatable>().CalculatePosition(game);

            // assert
            Assert.Equal(1, exactLocation.Trait<Hospitality>().ActorIds.Count);
            Assert.Equal(exactLocation.Id, scout.Trait<Locatable>().HostLocationActorId);
            Assert.False(scout.Trait<Locatable>().HasOwnPosition);
            Assert.Equal(10, coordinate.X);
            Assert.Equal(10, coordinate.Y);
        }
    }
}