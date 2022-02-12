using GreenStar.Resources;

namespace GreenStar.Research;

public record Technology(
    string Name,
    string DisplayName, string Description,
    bool IsVisible, // the technology (and its sub tree will never be visible)
    bool IsBaseDiscovered, // is the technology existence discovered at the player start (can be known future tech, can be used tech without knowing it)
    bool CanBeResearched, // can the user improve the technology by doing research (otherwise must be e.g. gifted)
    int InitialLevel, int MinLevel, int? MaxLevel, // level 0 => not achieved
    bool AllowCustomTechnologyChilds = false, // allow user-specific technology developments (e.g. ship blueprints)
    string? TechnologyData = null, // generic data store
    TechnologyEvent? LevelUpEvent = null, // event fired if the next level is achieved
    AnnotatedTechnologyLevel[]? AnnotatedLevels = null, // 
    string[]? RequiredTechnologies = null,
    string[]? BlockingTechnologies = null,
    Technology[]? ChildTechnologies = null);

public record AnnotatedTechnologyLevel(int Level, string DisplayName, ResourceAmount? ResourcesThreshold, TechnologyEvent? Event);
public record TechnologyEvent(string Type, string[] Arguments, string Text);

public record PlayerTechnologyState(
    int CurrentIncomePercentage,
    Technology[] Technologies,
    TechnologyProgress[] Progress
);
public record TechnologyProgress(
    string Name,
    bool IsDiscovered, int CurrentLevel, ResourceAmount ResourcesSpent, ResourceAmount ResourcesThreshold,
    double CurrentResearchBudgetPercentage);


public record TechnologyLevelUp(Technology Technology, TechnologyProgress Progress);
