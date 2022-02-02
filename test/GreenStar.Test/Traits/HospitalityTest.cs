using System;

using GreenStar.Core.Cartography;
using GreenStar.Ships;
using GreenStar.Stellar;

using Xunit;

namespace GreenStar.Core.Traits;

public class HospitalityTest
{
    [Fact]
    public void Hospitality_Enter_Simple()
    {
        // arrange
        var exactLocation = new ExactLocation();
        exactLocation.Trait<Locatable>().Position = new Coordinate(10, 10);

        var scout = new Scout();

        // act
        exactLocation.Trait<Hospitality>().Enter(scout);

        // assert
        Assert.Equal(1, exactLocation.Trait<Hospitality>().ActorIds.Count);
        Assert.Equal(exactLocation.Id, scout.Trait<Locatable>().HostLocationActorId);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
    }

    [Fact]
    public void Hospitality_Leave_Simple()
    {
        // arrange
        var exactLocation = new ExactLocation();
        exactLocation.Trait<Locatable>().Position = new Coordinate(10, 10);

        var scout = new Scout();
        exactLocation.Trait<Hospitality>().Enter(scout);

        // act
        exactLocation.Trait<Hospitality>().Leave(scout);

        // assert
        Assert.Equal(0, exactLocation.Trait<Hospitality>().ActorIds.Count);
        Assert.Equal(Guid.Empty, scout.Trait<Locatable>().HostLocationActorId);
        Assert.True(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(10, scout.Trait<Locatable>().Position.X);
        Assert.Equal(10, scout.Trait<Locatable>().Position.Y);
    }
}
