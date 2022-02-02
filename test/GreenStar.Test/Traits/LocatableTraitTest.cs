using System;

using GreenStar.Cartography;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.TurnEngine;

using Xunit;

namespace GreenStar.Traits;

public class LocatableTest
{
    [Fact]
    public void Locatable_Leave_Simple()
    {
        // arrange
        var exactLocation = new ExactLocation();
        exactLocation.Trait<Locatable>().Position = new Coordinate(10, 10);

        var scout = new Scout();
        exactLocation.Trait<Hospitality>().Enter(scout);

        var turnManager = new TurnManagerBuilder()
            .AddActor(exactLocation)
            .AddActor(scout)
            .Build();

        var context = turnManager.CreateTurnContext();

        // act
        var coordinate = exactLocation.Trait<Locatable>().CalculatePosition(context.ActorContext);

        // assert
        Assert.Equal(1, exactLocation.Trait<Hospitality>().ActorIds.Count);
        Assert.Equal(exactLocation.Id, scout.Trait<Locatable>().HostLocationActorId);
        Assert.False(scout.Trait<Locatable>().HasOwnPosition);
        Assert.Equal(10, coordinate.X);
        Assert.Equal(10, coordinate.Y);
    }
}
