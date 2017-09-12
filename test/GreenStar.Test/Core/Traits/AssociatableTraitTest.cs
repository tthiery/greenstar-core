using System;
using Xunit;

namespace GreenStar.Core.Traits
{
    public class AssociatableTraitTest
    {
        [Fact]
        public void AssociatableTrait_IsOwnedByAnyPlayer_HasPlayerId()
        {
            // arrange
            var trait = new AssociatableTrait();

            trait.PlayerId = Guid.NewGuid();

            // act
            bool result = trait.IsOwnedByAnyPlayer();

            // assert
            Assert.True(result);
        }

        [Fact]
        public void AssociatableTrait_IsOwnedByAnyPlayer_HasNoPlayerId()
        {
            // arrange
            var trait = new AssociatableTrait();

            trait.PlayerId = Guid.Empty;

            // act
            bool result = trait.IsOwnedByAnyPlayer();

            // assert
            Assert.False(result);
        }

        [Fact]
        public void AssociatableTrait_IsOwnedByAnyPlayer_Match()
        {
            // arrange
            var p = Guid.NewGuid();

            var trait = new AssociatableTrait();

            trait.PlayerId = p;

            // act
            bool result = trait.IsOwnedByPlayer(p);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void AssociatableTrait_IsOwnedByAnyPlayer_NoMatch()
        {
            // arrange
            var p = Guid.NewGuid();

            var trait = new AssociatableTrait();

            trait.PlayerId = Guid.NewGuid();

            // act
            bool result = trait.IsOwnedByPlayer(p);

            // assert
            Assert.False(result);
        }
    }
}