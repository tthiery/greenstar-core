using System;
using GreenStar.Core.Cartography;
using GreenStar.Ships;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class HostTraitTest
    {
        [Fact]
        public void HostTrait_Enter_Simple()
        {
            // arrange
            var exactLocation = new ExactLocation(Guid.NewGuid());
            exactLocation.Trait<LocatableTrait>().Position = new Coordinate(10, 10);

            var scout = new Scout(Guid.NewGuid());

            // act
            exactLocation.Trait<HostTrait>().Enter(scout);

            // assert
            Assert.Equal(1, exactLocation.Trait<HostTrait>().ActorIds.Count);
            Assert.Equal(exactLocation.Id, scout.Trait<LocatableTrait>().HostLocationActorId);
            Assert.False(scout.Trait<LocatableTrait>().HasOwnPosition);
        }

        [Fact]
        public void HostTrait_Leave_Simple()
        {
            // arrange
            var exactLocation = new ExactLocation(Guid.NewGuid());
            exactLocation.Trait<LocatableTrait>().Position = new Coordinate(10, 10);

            var scout = new Scout(Guid.NewGuid());
            exactLocation.Trait<HostTrait>().Enter(scout);

            // act
            exactLocation.Trait<HostTrait>().Leave(scout);

            // assert
            Assert.Equal(0, exactLocation.Trait<HostTrait>().ActorIds.Count);
            Assert.Equal(Guid.Empty, scout.Trait<LocatableTrait>().HostLocationActorId);
            Assert.True(scout.Trait<LocatableTrait>().HasOwnPosition);
            Assert.Equal(10, scout.Trait<LocatableTrait>().Position.X);
            Assert.Equal(10, scout.Trait<LocatableTrait>().Position.Y);
        }
    }
}