using System;
using GreenStar.Stellar;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class DiscoverableTraitTest
    {
        [Fact]
        public void DiscoverableTrait_AddDiscoverer_Simple()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());

            // act
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 23);

            // assert
            var entry = location.Trait<DiscoverableTrait>().RetrieveDiscoveryEntry(p1);
            Assert.Equal(p1, entry.PlayerId);
            Assert.Equal(DiscoveryLevel.LocationAware, entry.Level);
            Assert.Equal(23, entry.Turn);
        }

        [Fact]
        public void DiscoverableTrait_AddDiscoverer_ChangeOnSamePlayer()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 23);

            // act
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // assert
            var entry = location.Trait<DiscoverableTrait>().RetrieveDiscoveryEntry(p1);
            Assert.Equal(p1, entry.PlayerId);
            Assert.Equal(DiscoveryLevel.PropertyAware, entry.Level);
            Assert.Equal(24, entry.Turn);
        }

        [Fact]
        public void DiscoverableTrait_AddDiscoverer_MultiplePlayer()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 33);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);

            // act
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // assert
            var entry = location.Trait<DiscoverableTrait>().RetrieveDiscoveryEntry(p1);
            Assert.Equal(p1, entry.PlayerId);
            Assert.Equal(DiscoveryLevel.PropertyAware, entry.Level);
            Assert.Equal(24, entry.Turn);
        }

        [Fact]
        public void DiscoverableTrait_RequiredTurnSnapshots_Get()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // act
            var result = location.Trait<DiscoverableTrait>().RequiredTurnSnapshots;

            // assert
            Assert.Equal(24, result[0]);
            Assert.Equal(43, result[1]);
        }

        [Fact]
        public void DiscoverableTrait_RetrieveDiscoveryLevel_Existing()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // act
            var result = location.Trait<DiscoverableTrait>().RetrieveDiscoveryLevel(p1);

            // assert
            Assert.Equal(DiscoveryLevel.PropertyAware, result);
        }

        [Fact]
        public void DiscoverableTrait_RetrieveDiscoveryLevel_DefaultToUnknown()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var p4 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);

            // act
            var result = location.Trait<DiscoverableTrait>().RetrieveDiscoveryLevel(p4);

            // assert
            Assert.Equal(DiscoveryLevel.Unknown, result);
        }

        
        [Fact]
        public void DiscoverableTrait_RetrieveDiscoveryLevel_BestLevel()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var p2 = Guid.NewGuid();
            var p3 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(p3, DiscoveryLevel.LocationAware, 43);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p2, DiscoveryLevel.LocationAware, 24);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.PropertyAware, 24);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 44);

            // act
            var result = location.Trait<DiscoverableTrait>().RetrieveDiscoveryLevel(p1);

            // assert
            Assert.Equal(DiscoveryLevel.PropertyAware, result);
        }

        [Fact]
        public void DiscoverableTrait_RetrieveDiscoveryLevel_Everyone()
        {
            // arrange
            var p1 = Guid.NewGuid();
            var location = new ExactLocation(Guid.NewGuid());
            location.Trait<DiscoverableTrait>().AddDiscoverer(Guid.Empty, DiscoveryLevel.PropertyAware, 43);
            location.Trait<DiscoverableTrait>().AddDiscoverer(p1, DiscoveryLevel.LocationAware, 44);

            // act
            var result = location.Trait<DiscoverableTrait>().RetrieveDiscoveryLevel(p1);

            // assert
            Assert.Equal(DiscoveryLevel.PropertyAware, result);
        }
    }
}