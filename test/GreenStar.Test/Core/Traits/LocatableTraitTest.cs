using System;
using GreenStar.Core.Cartography;
using GreenStar.Core.TurnEngine;
using GreenStar.Ships;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class LocatableTraitTest
    {
        [Fact]
        public void LocatableTrait_Leave_Simple()
        {
            // arrange
            var exactLocation = new ExactLocation(Guid.NewGuid());
            exactLocation.Trait<LocatableTrait>().Position = new Coordinate(10, 10);

            var scout = new Scout(Guid.NewGuid());
            exactLocation.Trait<HostTrait>().Enter(scout);

            var game = new Game(Guid.NewGuid(), new Player[0], new Actor[] { exactLocation, scout });

            // act
            var coordinate = exactLocation.Trait<LocatableTrait>().CalculatePosition(game);

            // assert
            Assert.Equal(1, exactLocation.Trait<HostTrait>().ActorIds.Count);
            Assert.Equal(exactLocation.Id, scout.Trait<LocatableTrait>().HostLocationActorId);
            Assert.False(scout.Trait<LocatableTrait>().HasOwnPosition);
            Assert.Equal(10, coordinate.X);
            Assert.Equal(10, coordinate.Y);
        }
    }
}