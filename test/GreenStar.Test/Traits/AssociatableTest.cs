using System;

using Xunit;

namespace GreenStar.Traits;

public class AssociatableTest
{
    [Fact]
    public void Associatable_IsOwnedByAnyPlayer_HasPlayerId()
    {
        // arrange
        var trait = new Associatable();

        trait.PlayerId = Guid.NewGuid();

        // act
        bool result = trait.IsOwnedByAnyPlayer();

        // assert
        Assert.True(result);
    }

    [Fact]
    public void Associatable_IsOwnedByAnyPlayer_HasNoPlayerId()
    {
        // arrange
        var trait = new Associatable();

        trait.PlayerId = Guid.Empty;

        // act
        bool result = trait.IsOwnedByAnyPlayer();

        // assert
        Assert.False(result);
    }

    [Fact]
    public void Associatable_IsOwnedByAnyPlayer_Match()
    {
        // arrange
        var p = Guid.NewGuid();

        var trait = new Associatable();

        trait.PlayerId = p;

        // act
        bool result = trait.IsOwnedByPlayer(p);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void Associatable_IsOwnedByAnyPlayer_NoMatch()
    {
        // arrange
        var p = Guid.NewGuid();

        var trait = new Associatable();

        trait.PlayerId = Guid.NewGuid();

        // act
        bool result = trait.IsOwnedByPlayer(p);

        // assert
        Assert.False(result);
    }
}
