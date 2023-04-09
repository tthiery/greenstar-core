using System;

using static GreenStar.Test.Helper;

using Xunit;

namespace GreenStar.TurnEngine;

public class PlayerTest
{
    [Fact]
    public void Player_IsFriendlyTo_True()
    {
        // arrange
        var p1Guid = Guid.NewGuid();
        var p2Guid = Guid.NewGuid();

        var p1 = CreateHumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
        var p2 = CreateHumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

        // act
        var isFriendly = p1.Relatable.IsFriendlyTo(p2);

        // assert
        Assert.True(isFriendly);
    }

    [Fact]
    public void Player_IsFriendlyTo_False()
    {
        // arrange
        var p1Guid = Guid.NewGuid();
        var p2Guid = Guid.NewGuid();

        var p1 = CreateHumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
        var p2 = CreateHumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

        // act
        var isFriendly = p2.Relatable.IsFriendlyTo(p1);

        // assert
        Assert.False(isFriendly);
    }

    [Fact]
    public void Player_IsFriendlyToByGuid_True()
    {
        // arrange
        var p1Guid = Guid.NewGuid();
        var p2Guid = Guid.NewGuid();

        var p1 = CreateHumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
        var p2 = CreateHumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

        // act
        var isFriendly = p1.Relatable.IsFriendlyTo(p2Guid);

        // assert
        Assert.True(isFriendly);
    }

    [Fact]
    public void Player_IsFriendlyToByGuid_False()
    {
        // arrange
        var p1Guid = Guid.NewGuid();
        var p2Guid = Guid.NewGuid();

        var p1 = CreateHumanPlayer(p1Guid, "Red", new Guid[] { p2Guid }, 22, 1);
        var p2 = CreateHumanPlayer(p2Guid, "Blue", new Guid[0], 22, 1);

        // act
        var isFriendly = p2.Relatable.IsFriendlyTo(p1Guid);

        // assert
        Assert.False(isFriendly);
    }
}
