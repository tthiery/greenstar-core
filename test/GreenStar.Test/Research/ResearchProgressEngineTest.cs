using GreenStar.Core.Resources;

using Xunit;

namespace GreenStar.Research;

public class InMemoryTechnologyDefinitionLoader : ITechnologyDefinitionLoader
{
    public Technology[] GetTechnologiesByDefinition(string technologyDefinition)
        => new[] {
            new Technology("A", "Public Tech", "", true, true, true, 6, 0, null, false, null, null, null, null, null, null),
            new Technology("B", "Hidden Tech", "", false, true, true, 6, 0, 7, false, null, null, null, null, null, null),
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

    [Fact]
    public void ResearchProgressEngine_InvestInTechnology_NoLevelUps()
    {
        // arrange
        var inMemoryTechnologyDefinitionLoader = new InMemoryTechnologyDefinitionLoader();
        var researchManager = new ResearchProgressEngine(inMemoryTechnologyDefinitionLoader);
        var state = researchManager.CreateTechnologyStateForPlayer("init-setup");
        ResourceAmount investment = "Money: 4000";

        // act
        (state, var levelUps, _) = researchManager.InvestInTechnology(state, investment);

        // assert
        Assert.Collection(state.Progress,
            p => { Assert.Equal("A", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(50, p.CurrentResearchBudgetPercentage); Assert.Equal("Money: 2000", p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("B", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(50, p.CurrentResearchBudgetPercentage); Assert.Equal("Money: 2000", p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("C", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("E", p.Name); Assert.False(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("F", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(0, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); }
        );
        Assert.Empty(levelUps);

    }
    [Fact]
    public void ResearchProgressEngine_InvestInTechnology_TwoLevelUps()
    {
        // arrange
        var inMemoryTechnologyDefinitionLoader = new InMemoryTechnologyDefinitionLoader();
        var researchManager = new ResearchProgressEngine(inMemoryTechnologyDefinitionLoader);
        var state = researchManager.CreateTechnologyStateForPlayer("init-setup");
        ResourceAmount investment = "Money: 14000";

        // act
        (state, var levelUps, var remainingBudget) = researchManager.InvestInTechnology(state, investment);

        // assert
        Assert.Equal("Money: 1000", remainingBudget.ToString());
        Assert.Collection(state.Progress,
            p => { Assert.Equal("A", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(7, p.CurrentLevel); Assert.Equal(100, p.CurrentResearchBudgetPercentage); Assert.Equal("Money: 1000", p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("B", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(7, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("C", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("E", p.Name); Assert.False(p.IsDiscovered); Assert.Equal(6, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); },
            p => { Assert.Equal("F", p.Name); Assert.True(p.IsDiscovered); Assert.Equal(0, p.CurrentLevel); Assert.Equal(0, p.CurrentResearchBudgetPercentage); Assert.Equal(string.Empty, p.ResourcesSpent.ToString()); }
        );
        Assert.Collection(levelUps,
            up => { Assert.Equal("A", up.Progress.Name); Assert.Equal(7, up.Progress.CurrentLevel); },
            up => { Assert.Equal("B", up.Progress.Name); Assert.Equal(7, up.Progress.CurrentLevel); }
        );

    }
}