using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Core;
using GreenStar.Core.Resources;

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

        var state = new PlayerTechnologyState(technologies, result);

        // initialize budget
        state = AdjustBudget(state);

        return state;
    }

    public (PlayerTechnologyState, TechnologyLevelUp[]) InvestInTechnology(Context context, PlayerTechnologyState state, ResourceAmount moneySpentOnResearch)
    {
        var levelUps = new List<TechnologyLevelUp>();

        var remainingBudget = moneySpentOnResearch;

        // divide money into technology progress items
        var researchableTechnologies = state.Progress.Where(p => CanUserInvestInTechnology(state, FindTechnologyByName(state, p.Name) ?? throw new InvalidOperationException("progress without technology"), p)).ToArray();

        // progress each technology
        foreach (var progress in researchableTechnologies)
        {
            var technology = FindTechnologyByName(state, progress.Name) ?? throw new InvalidOperationException("progress without technology");
            var newProgress = progress;

            // ... calculate invest in technology
            var invest = moneySpentOnResearch * (progress.CurrentResearchBudgetPercentage / 100);

            var newThreshold = progress.ResourcesThreshold;
            var newSpent = progress.ResourcesSpent + invest;

            // ... calculate if threshold is met & set new threshold
            if (newSpent > progress.ResourcesThreshold)
            {
                // level up (incl side effects and events, etc)
                newProgress = IncreaseLevel(context, state, technology, newProgress);

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

                levelUps.Add(new TechnologyLevelUp(technology, newProgress));
            }

            newProgress = newProgress with
            {
                ResourcesSpent = newSpent,
            };

            state = state.WithProgress(newProgress);
        }

        // technologies might have maxed out
        state = AdjustBudget(state);

        return (state, levelUps.ToArray());
    }

    public TechnologyProgress IncreaseLevel(Context context, PlayerTechnologyState state, Technology technology, TechnologyProgress progress)
    {
        // check if max level . return
        if (technology.MaxLevel == null || progress.CurrentLevel < technology.MaxLevel)
        {
            return progress;
        }
        // set new level
        int newLevel = progress.CurrentLevel + 1;
        progress = progress with
        {
            CurrentLevel = newLevel,

            // set discovered if first level (otherwise keep as is)
            IsDiscovered = progress is { CurrentLevel: 0 } ? true : progress.IsDiscovered,
            // set budget to zero if max level
            CurrentResearchBudgetPercentage = (progress.CurrentLevel >= technology.MaxLevel) ? 0 : progress.CurrentResearchBudgetPercentage,
            // calculate new threshold
            ResourcesThreshold = DetermineNewThreshold(technology, newLevel),
        };

        return progress;
    }

    private ResourceAmount DetermineNewThreshold(Technology technology, int newLevel)
        => technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == newLevel) switch
        {
            { ResourcesThreshold: not null } and var annotatedLevel => annotatedLevel.ResourcesThreshold,
            _ => new ResourceAmount("Money", new[] { new ResourceAmountItem("Money", newLevel * 1000) }),
        };

    public Technology? FindTechnologyByName(PlayerTechnologyState state, string name)
        => FindTechnologyByName(state.Technologies, name);

    public Technology? FindTechnologyByName(Technology[] technologies, string name)
    {
        foreach (var t in technologies)
        {
            if (t.Name == name)
            {
                return t;
            }

            if (t is { ChildTechnologies: not null })
            {
                var result = FindTechnologyByName(t.ChildTechnologies, name);

                if (result is not null)
                {
                    return result;
                }
            }
        }

        return null;
    }
    public bool CanUserSeeTechnology(Technology technology, TechnologyProgress progress)
        => !technology.IsVisible
            && progress.IsDiscovered;

    public bool CanUserInvestInTechnology(PlayerTechnologyState state, Technology technology, TechnologyProgress progress)
        => technology.CanBeResearched
            && (technology.RequiredTechnologies?.All(t => state.HasAchievedTechnologyLevel(t, 1)) ?? true)
            && !(technology.BlockingTechnologies?.Any(t => state.HasAchievedTechnologyLevel(t, 1)) ?? false)
            && progress.IsDiscovered
            && (technology.MaxLevel == null || progress.CurrentLevel < technology.MaxLevel);

    private void InitializeProgressList(List<TechnologyProgress> list, Technology[] technologies)
    {
        foreach (var technology in technologies)
        {
            if (technology is { IsBaseDiscovered: true } or { InitialLevel: > 0 })
            {
                list.Add(new TechnologyProgress(technology.Name, technology.IsBaseDiscovered, technology.InitialLevel, ResourceAmount.Empty, ResourceAmount.Empty, 0));
            }

            if (technology.ChildTechnologies is not null)
            {
                InitializeProgressList(list, technology.ChildTechnologies);
            }
        }
    }

    private PlayerTechnologyState AdjustBudget(PlayerTechnologyState state)
    {
        var list = state.Progress;
        var remainingBudget = 100 - state.Progress.Sum(x => x.CurrentResearchBudgetPercentage);
        var researchableTechnologies = state.Progress.Where(p => CanUserInvestInTechnology(state, FindTechnologyByName(state, p.Name) ?? throw new InvalidOperationException("progress without technology"), p)).ToArray();

        var budgetIncreaseByItem = remainingBudget / researchableTechnologies.Count();

        for (int i = 0; i < list.Length; i++)
        {
            var item = list[i];
            if (researchableTechnologies.Any(t => t.Name == item.Name))
            {
                item = item with
                {
                    CurrentResearchBudgetPercentage = budgetIncreaseByItem,
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
}