using System;
using System.Threading.Tasks;

using GreenStar.Algorithms;
using GreenStar.Stellar;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Traits;

public class OrbitingTest
{
    [Theory]
    [InlineData(0, 100, 0)]
    [InlineData(1, 0, 100)]
    [InlineData(2, -100, 0)]
    [InlineData(3, 0, -100)]
    [InlineData(4, 100, 0)]
    [InlineData(5, 0, 100)]
    public async Task Orbiting_Simple(int turns, long x, long y)
    {
        // arrange
        var sun = new Sun();
        sun.Trait<Locatable>().SetPosition((0, 0));
        var planet = new Planet();
        var orbiting = planet.Trait<Orbiting>();
        orbiting.Host = sun.Id;
        orbiting.Distance = 100;
        orbiting.SpeedDegree = 90;
        orbiting.CurrentDegree = 0;
        planet.Trait<Locatable>().SetPosition((100, 0));

        var turnManager = await new TurnManagerBuilder(new ServiceCollection()
            .Configure<PlanetLifeOptions>(_ => { })
            .BuildServiceProvider())
            .AddStellarTranscript()
            .AddActor(sun)
            .AddActor(planet)
            .BuildAsync();

        // act
        for (int i = 0; i < turns; i++)
        {
            await turnManager.StartRoundAsync();
        }

        // assert
        Assert.Equal((x, y), planet.Trait<Locatable>().GetPosition(null));
        Assert.Equal((turns * 90) % 360, orbiting.CurrentDegree);
    }
}
