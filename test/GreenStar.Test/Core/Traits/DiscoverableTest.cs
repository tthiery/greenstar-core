using System;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class DiscoverableTraitTest
    {
        [Fact]
        public void Discoverable_AddDiscoverer_Simple()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());

            // act
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 23);

            // assert
            var entry = location.Trait<Discoverable>().RetrieveDiscoveryEntry(p1);
            Assert.Equal(p1, entry.PlayerId);
            Assert.Equal(DiscoveryLevel.LocationAware, entry.Level);
            Assert.Equal(23, entry.Turn);
        }

        [Fact]
        public void Discoverable_AddDiscoverer_ChangeOnSamePlayer()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 23);

            // act
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // assert
            var entry = location.Trait<Discoverable>().RetrieveDiscoveryEntry(p1);
            Assert.Equal(p1, entry.PlayerId);
            Assert.Equal(DiscoveryLevel.PropertyAware, entry.Level);
            Assert.Equal(24, entry.Turn);
        }

        [Fact]
        public void Discoverable_AddDiscoverer_MultiplePlayer()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 33);
            location.Trait<Discoverable>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);

            // act
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // assert
            var entry = location.Trait<Discoverable>().RetrieveDiscoveryEntry(p1);
            Assert.Equal(p1, entry.PlayerId);
            Assert.Equal(DiscoveryLevel.PropertyAware, entry.Level);
            Assert.Equal(24, entry.Turn);
        }

        [Fact]
        public void Discoverable_RequiredTurnSnapshots_Get()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<Discoverable>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // act
            var result = location.Trait<Discoverable>().RequiredTurnSnapshots;

            // assert
            Assert.Equal(24, result[0]);
            Assert.Equal(43, result[1]);
        }

        [Fact]
        public void Discoverable_RetrieveDiscoveryLevel_Existing()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<Discoverable>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // act
            var result = location.Trait<Discoverable>().RetrieveDiscoveryLevel(p1);

            // assert
            Assert.Equal(DiscoveryLevel.PropertyAware, result);
        }

        [Fact]
        public void Discoverable_RetrieveDiscoveryLevel_DefaultToUnknown()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var p4 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<Discoverable>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // act
            var result = location.Trait<Discoverable>().RetrieveDiscoveryLevel(p4);

            // assert
            Assert.Equal(DiscoveryLevel.Unknown, result);
        }

        
        [Fact]
        public void Discoverable_RetrieveDiscoveryLevel_BestLevel()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<Discoverable>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 44);

            // act
            var result = location.Trait<Discoverable>().RetrieveDiscoveryLevel(p1);

            // assert
            Assert.Equal(DiscoveryLevel.PropertyAware, result);
        }

        [Fact]
        public void Discoverable_RetrieveDiscoveryLevel_Everyone()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<Discoverable>().AddDiscoverer(Guid.Empty, DiscoveryLevel.PropertyAware, 43);
            location.Trait<Discoverable>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 44);

            // act
            var result = location.Trait<Discoverable>().RetrieveDiscoveryLevel(p1);

            // assert
            Assert.Equal(DiscoveryLevel.PropertyAware, result);
        }
    }
}