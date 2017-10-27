using System;
using GreenStar.Ships;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class CapableTest
    {
        [Fact]
        public void Capable_Of_NoArbitraryCapabilies()
        {
            // arrange
            var scout = new Scout();

            scout.Trait<Capable>().Of("foo", 12);

            // act
            var result = scout.Trait<Capable>().Of("foo");

            // assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Capable_Of_Simple()
        {
            // arrange
            var scout = new Scout();

            scout.Trait<Capable>().Of(ShipCapabilities.Speed, 12);

            // act
            var result = scout.Trait<Capable>().Of(ShipCapabilities.Speed);

            // assert
            Assert.Equal(12, result);
        }

        [Fact]
        public void Capable_Of_Default()
        {
            // arrange
            var scout = new Scout();

            // act
            var result = scout.Trait<Capable>().Of(ShipCapabilities.Speed);

            // assert
            Assert.Equal(0, result);
        }
    }
}