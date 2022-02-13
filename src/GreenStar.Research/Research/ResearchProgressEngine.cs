using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar;
using GreenStar.Resources;

namespace GreenStar.Research;

public class ResearchProgressEngine
{
    private readonly ITechnologyDefinitionLoader _technologyDefinitionStore;

    public ResearchProgressEngine(ITechnologyDefinitionLoader technologyDefinitionStore)
    {
        _technologyDefinitionStore = technologyDefinitionStore;
    }
    public PlayerTechnologyState CreateTechnologyStateForPlayer(string technologyDefinition)
    {
        var technologies = _technologyDefinitionStore.GetTechnologiesByDefinition(technologyDefinition);

        // clone initial technologies into progress status
        var list = new List<TechnologyProgress>();
        InitializeProgressList(list, technologies);

        var result = list.ToArray();

        var state = new PlayerTechnologyState(50, technologies, result);

        // initialize budget
        state = AdjustBudget(state);

        return state;
    }
    private void InitializeProgressList(List<TechnologyProgress> list, Technology[] technologies)
    {
        foreach (var technology in technologies)
        {
            if (technology is { IsBaseDiscovered: true } or { InitialLevel: > 0 })
            {
                list.Add(new TechnologyProgress(technology.Name, technology.IsBaseDiscovered, technology.InitialLevel, ResourceAmount.Empty, DetermineNewThreshold(technology, technology.InitialLevel), 0));
            }

            if (technology.ChildTechnologies is not null)
            {
                InitializeProgressList(list, technology.ChildTechnologies);
            }
        }
    }

    public (PlayerTechnologyState, TechnologyLevelUp[], ResourceAmount) InvestInTechnology(PlayerTechnologyState state, ResourceAmount moneySpentOnResearch)
    {
        var levelUps = new List<TechnologyLevelUp>();

        var remainingBudget = ResourceAmount.Empty;

        // divide money into technology progress items
        var researchableTechnologies = state.Progress.Where(p => CanUserInvestInTechnology(state, state.FindTechnologyByName(p.Name) ?? throw new InvalidOperationException("progress without technology"), p)).ToArray();

        // progress each technology
        foreach (var progress in researchableTechnologies)
        {
            var technology = state.FindTechnologyByName(progress.Name) ?? throw new InvalidOperationException("progress without technology");
            var newProgress = progress;

            // ... calculate invest in technology
            var invest = moneySpentOnResearch * (progress.CurrentResearchBudgetPercentage / 100);

            var newThreshold = progress.ResourcesThreshold;
            var newSpent = progress.ResourcesSpent + invest;

            // ... calculate if threshold is met & set new threshold
            if (newSpent > progress.ResourcesThreshold)
            {
                // level up
                newProgress = IncreaseLevelInternal(state, technology, newProgress, 1);

                if (newProgress is not null)
                {
                    // if still investable, continue invest carry over
                    var leftover = newSpent - progress.ResourcesThreshold;
                    if (CanUserInvestInTechnology(state, technology, newProgress))
                    {
                        newSpent = leftover;
                    }
                    else
                    {
                        remainingBudget += leftover;

                        newSpent = ResourceAmount.Empty;
                    }

                    levelUps.Add(new TechnologyLevelUp(technology, newProgress, progress));
                }
            }

            newProgress = newProgress with
            {
                ResourcesSpent = newSpent,
            };

            state = state.WithProgress(newProgress);
        }

        // technologies might have maxed out
        state = AdjustBudget(state);

        return (state, levelUps.ToArray(), remainingBudget);
    }

    public (PlayerTechnologyState, TechnologyLevelUp?) IncreaseLevel(PlayerTechnologyState state, string technologyName, int levelIncrease)
    {
        var technology = state.FindTechnologyByName(technologyName) ?? throw new ArgumentException("unknown technology", nameof(technologyName));
        var progress = state.Progress.FirstOrDefault(p => p.Name == technologyName) ?? throw new InvalidOperationException("progress without technology");
        var oldLevel = progress.CurrentLevel;

        var newProgress = IncreaseLevelInternal(state, technology, progress, levelIncrease);

        if (newProgress is not null)
        {
            // reset spent money to zero ... since external progress happened
            newProgress = newProgress with
            {
                ResourcesSpent = ResourceAmount.Empty,
            };

            state = state.WithProgress(newProgress);

            // technologies might have maxed out
            state = AdjustBudget(state);

            return (state, new TechnologyLevelUp(technology, newProgress, progress));
        }
        else
        {
            return (state, null);
        }
    }

    private TechnologyProgress? IncreaseLevelInternal(PlayerTechnologyState state, Technology technology, TechnologyProgress progress, int levelIncrease)
    {
        // check if max level . return
        if (technology.MaxLevel is not null && progress.CurrentLevel >= technology.MaxLevel)
        {
            return null;
        }
        // set new level
        int newLevel = progress.CurrentLevel + levelIncrease;
        newLevel = (technology.MaxLevel is not null && newLevel > technology.MaxLevel) ? technology.MaxLevel.Value : newLevel;

        progress = progress with
        {
            CurrentLevel = newLevel,

            // set discovered if first level (otherwise keep as is)
            IsDiscovered = progress is { CurrentLevel: 0 } ? true : progress.IsDiscovered,
            // set budget to zero if max level
            CurrentResearchBudgetPercentage = (newLevel >= technology.MaxLevel) ? 0 : progress.CurrentResearchBudgetPercentage,
            // calculate new threshold
            ResourcesThreshold = (newLevel >= technology.MaxLevel) ? ResourceAmount.Empty : DetermineNewThreshold(technology, newLevel),
        };

        return progress;
    }

    private ResourceAmount DetermineNewThreshold(Technology technology, int newLevel)
        => technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == newLevel) switch
        {
            { ResourcesThreshold: not null } and var annotatedLevel => annotatedLevel.ResourcesThreshold,
            _ => new ResourceAmount("Money", new[] { new ResourceAmountItem("Money", newLevel * 1000) }),
        };

    public static bool CanUserSeeTechnology(Technology technology, TechnologyProgress progress)
        => !technology.IsVisible
            && progress.IsDiscovered;

    public static bool CanUserInvestInTechnology(PlayerTechnologyState state, Technology technology, TechnologyProgress progress)
        => technology.CanBeResearched
            && (technology.RequiredTechnologies?.All(t => state.HasAchievedTechnologyLevel(t, 1)) ?? true)
            && !(technology.BlockingTechnologies?.Any(t => state.HasAchievedTechnologyLevel(t, 1)) ?? false)
            && progress.IsDiscovered
            && (technology.MaxLevel is null || progress.CurrentLevel < technology.MaxLevel);

    private PlayerTechnologyState AdjustBudget(PlayerTechnologyState state)
    {
        var list = state.Progress;
        var remainingBudget = 100 - state.Progress.Sum(x => x.CurrentResearchBudgetPercentage);

        if (remainingBudget > 0)
        {
            var researchableTechnologies = state.Progress.Where(p => CanUserInvestInTechnology(state, state.FindTechnologyByName(p.Name) ?? throw new InvalidOperationException("progress without technology"), p)).ToArray();

            var budgetIncreaseByItem = remainingBudget / researchableTechnologies.Count();

            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];
                if (researchableTechnologies.Any(t => t.Name == item.Name))
                {
                    item = item with
                    {
                        CurrentResearchBudgetPercentage = item.CurrentResearchBudgetPercentage + budgetIncreaseByItem,
                    };

                    remainingBudget -= item.CurrentResearchBudgetPercentage;
                }
                else
                {
                    item = item with
                    {
                        CurrentResearchBudgetPercentage = 0,
                    };
                }

                list[i] = item;
            }

            // if rounding issue, just correct difference on first budget
            if (remainingBudget > 0)
            {
                list[0] = list[0] with
                {
                    CurrentResearchBudgetPercentage = list[0].CurrentResearchBudgetPercentage + remainingBudget,
                };
            }

            return state with
            {
                Progress = list,
            };
        }
        else
        {
            return state;
        }
    }
}