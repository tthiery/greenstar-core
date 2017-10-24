using System;
using GreenStar.Core.TurnEngine.Players;
using Xunit;

namespace GreenStar.Core.TurnEngine
{
    public class PlayerTest
    {
        [Fact]
        public void Player_IsFriendlyTo_True()
        {
            // arrange
            var p1Guid = Guid.NewGuid();
            var p2Guid = Guid.NewGuid();

            var p1 = new HumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
            var p2 = new HumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

            // act
            var isFriendly = p1.IsFriendlyTo(p2);

            // assert
            Assert.True(isFriendly);
        }

        [Fact]
        public void Player_IsFriendlyTo_False()
        {
            // arrange
            var p1Guid = Guid.NewGuid();
            var p2Guid = Guid.NewGuid();

            var p1 = new HumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
            var p2 = new HumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

            // act
            var isFriendly = p2.IsFriendlyTo(p1);

            // assert
            Assert.False(isFriendly);
        }

                [Fact]
        public void Player_IsFriendlyToByGuid_True()
        {
            // arrange
            var p1Guid = Guid.NewGuid();
            var p2Guid = Guid.NewGuid();

            var p1 = new HumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
            var p2 = new HumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

            // act
            var isFriendly = p1.IsFriendlyTo(p2Guid);

            // assert
            Assert.True(isFriendly);
        }

        [Fact]
        public void Player_IsFriendlyToByGuid_False()
        {
            // arrange
            var p1Guid = Guid.NewGuid();
            var p2Guid = Guid.NewGuid();

            var p1 = new HumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
            var p2 = new HumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

            // act
            var isFriendly = p2.IsFriendlyTo(p1Guid);

            // assert
            Assert.False(isFriendly);
        }
    }
}