using Xunit;

namespace GreenStar.Research;

public class InMemoryTechnologyDefinitionLoader : ITechnologyDefinitionLoader
{
    public Technology[] GetTechnologiesByDefinition(string technologyDefinition)
        => new[] {
            new Technology("A", "Public Tech", "", true, true, true, 6, 0, null, false, null, null, null, null, null, null),
            new Technology("B", "Hidden Tech", "", false, true, true, 6, 0, null, false, null, null, null, null, null, null),
            new Technology("C", "No Researchable", "", true, true, false, 6, 0, null, false, null, null, null, null, null, null),
            new Technology("D", "Not Base Discovered; No Level", "", true, false, true, 0, 0, null, false, null, null, null, null, null, null),
            new Technology("E", "Not Base Discovered; Some Level", "", true, false, true, 6, 0, null, false, null, null, null, null, null, null),
            new Technology("F", "Public; No Research; No Level", "", true, true, false, 0, 0, null, false, null, null, null, null, null, null),
        };
}

public class ResearchProgressEngineTest
{
    [Fact]
    public void ResearchProgressEngine_CreatePlayerState()
    {
        // arrange
        var inMemoryTechnologyDefinitionLoader = new InMemoryTechnologyDefinitionLoader();
        var researchManager = new ResearchProgressEngine(inMemoryTechnologyDefinitionLoader);

        // act
        var state = researchManager.CreateTechnologyStateForPlayer("init-setup");

        // assert
        Assert.Collection(state.Progress,
            p => { Assert.Equal("A", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(50, p.CurrentResearchBudgetPercentage); },
            p => { Assert.Equal("B", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(50, p.CurrentResearchBudgetPercentage); },
            p => { Assert.Equal("C", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); },
            p => { Assert.Equal("E", p.Name); Assert.False(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); },
            p => { Assert.Equal("F", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(0, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); }
        );
    }
}