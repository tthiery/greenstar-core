using System;
using System.Threading.Tasks;

using GreenStar.Research;
using GreenStar.TurnEngine;
using GreenStar.TurnEngine.Players;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace GreenStar.Transcripts;


public class TechnologyGiftEventTest
{
    public const string EventType = "GreenStar.Transcripts.TechnologyGiftEvent, GreenStar.Research";
    [Fact]
    public async Task TechnologyGiftEvent_Execute_Normal()
    {
        // arrange
        var (turnManager, p1, p2, p3, stateLoader) = await CreateEnvironmentAsync();

        var context = turnManager.CreateTurnContext(p1);
        var player1 = context.PlayerContext.GetPlayer(p1);

        // act
        await context.TurnContext.ExecuteEventAsync(context, player1, EventType, new[] { "A", "+2" }, "Happy");

        // assert
        var state = await stateLoader.LoadAsync(p1);
        Assert.Equal(2, state.GetTechnologyLevel("A"));
        Assert.Equal(2, player1.Capable.Of("A"));
        Assert.Equal(0, state.GetTechnologyLevel("B"));
        Assert.Equal(0, player1.Capable.Of("B"));
        Assert.Equal(0, state.GetTechnologyLevel("C"));
        Assert.Equal(0, player1.Capable.Of("C"));
    }
    [Fact]
    public async Task TechnologyGiftEvent_Execute_WithLevelUpEvents()
    {
        // arrange
        var (turnManager, p1, p2, p3, stateLoader) = await CreateEnvironmentAsync();

        var context = turnManager.CreateTurnContext(p1);
        var player1 = context.PlayerContext.GetPlayer(p1);

        // act
        await context.TurnContext.ExecuteEventAsync(context, player1, EventType, new[] { "B", "+3" }, "Happy");

        // assert
        var state = await stateLoader.LoadAsync(p1);
        Assert.Equal(0, state.GetTechnologyLevel("A"));
        Assert.Equal(0, player1.Capable.Of("A"));
        Assert.Equal(3, state.GetTechnologyLevel("B"));
        Assert.Equal(3, player1.Capable.Of("B"));
        Assert.Equal(120, state.GetTechnologyLevel("C"));
        Assert.Equal(120, player1.Capable.Of("C"));
    }

    public async Task<(TurnManager turnManager, Guid p1, Guid p2, Guid p3, IPlayerTechnologyStateLoader stateLoader)> CreateEnvironmentAsync()
    {
        var p1 = Guid.NewGuid();
        var p2 = Guid.NewGuid();
        var p3 = Guid.NewGuid();

        var sp = new ServiceCollection()
            .AddSingleton<IPlayerTechnologyStateLoader, InMemoryPlayerTechnologyStateLoader>()
            .AddSingleton<ITechnologyDefinitionLoader>(new InMemoryTechnologyDefinitionLoader(() => new[] {
                new Technology("A", "A", "A", true, true, true, 0, 0, null),
                new Technology("B", "A", "A", true, true, true, 0, 0, null, LevelUpEvent: new(EventType, new[] { "C", "+10" }, string.Empty), AnnotatedLevels: new AnnotatedTechnologyLevel[] {
                    new AnnotatedTechnologyLevel(3, string.Empty, new Resources.ResourceAmount(), new TechnologyEvent(EventType, new[] { "C", "+100"}, string.Empty))
                }),
                new Technology("C", "A", "A", true, true, true, 0, 0, null),
            }))
            .AddSingleton<ResearchProgressEngine>()
            .BuildServiceProvider();

        var turnManager = await new TurnManagerBuilder(sp)
            .AddTranscript<ResearchSetup>(TurnTranscriptGroups.Setup)
            .AddPlayer(new HumanPlayer(p1, "red", new Guid[] { p2 }, 20, 1))
            .AddPlayer(new HumanPlayer(p2, "blue", new Guid[] { p1 }, 20, 1))
            .AddPlayer(new HumanPlayer(p3, "orange", new Guid[] { }, 20, 1))
            .BuildAsync();

        return (turnManager, p1, p2, p3, sp.GetService<IPlayerTechnologyStateLoader>());
    }
}