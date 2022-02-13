using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Resources;

namespace GreenStar.Research;

public class ResearchProgressEngine
{
    private readonly TechnologyProgressEngine _technologyProgressEngine;

    public ResearchProgressEngine(TechnologyProgressEngine technologyProgressEngine)
    {
        _technologyProgressEngine = technologyProgressEngine ?? throw new ArgumentNullException(nameof(technologyProgressEngine));
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
            var newProgress = progress;

            var technology = state.FindTechnologyByName(progress.Name) ?? throw new InvalidOperationException("progress without technology");

            // ... calculate invest in technology
            var invest = moneySpentOnResearch * (progress.CurrentResearchBudgetPercentage / 100);

            var newThreshold = progress.ResourcesThreshold;
            var newSpent = progress.ResourcesSpent + invest;

            // ... calculate if threshold is met & set new threshold
            if (newSpent > progress.ResourcesThreshold)
            {
                // level up
                (state, var techLevelUp) = _technologyProgressEngine.IncreaseLevel(state, technology.Name, 1);

                if (techLevelUp?.Progress is not null)
                {
                    newProgress = techLevelUp.Progress;

                    // if still investable, continue invest carry over
                    var leftover = newSpent - progress.ResourcesThreshold;
                    if (CanUserInvestInTechnology(state, technology, techLevelUp.Progress))
                    {
                        newSpent = leftover;
                    }
                    else
                    {
                        remainingBudget += leftover;

                        newSpent = ResourceAmount.Empty;
                    }

                    levelUps.Add(techLevelUp);
                }
            }

            newProgress = newProgress with
            {
                ResourcesSpent = newSpent,
            };

            state = state.WithProgress(newProgress);
        }

        return (state, levelUps.ToArray(), remainingBudget);
    }

    private ResourceAmount DetermineNewThreshold(Technology technology, int newLevel)
        => technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == newLevel) switch
        {
            { ResourcesThreshold: not null } and var annotatedLevel => annotatedLevel.ResourcesThreshold,
            _ => new ResourceAmount("Money", new[] { new ResourceAmountItem("Money", newLevel * 1000) }),
        };

    public static bool CanUserInvestInTechnology(PlayerTechnologyState state, Technology technology, TechnologyProgress progress)
        => technology.CanBeResearched
            && (technology.RequiredTechnologies?.All(t => state.HasAchievedTechnologyLevel(t, 1)) ?? true)
            && !(technology.BlockingTechnologies?.Any(t => state.HasAchievedTechnologyLevel(t, 1)) ?? false)
            && progress.IsDiscovered
            && (technology.MaxLevel is null || progress.CurrentLevel < technology.MaxLevel);

    public PlayerTechnologyState AdjustBudgetAndDetermineThresholds(PlayerTechnologyState state)
    {
        var list = state.Progress;

        // zero out budgets for technology without future
        for (int i = 0; i < list.Length; i++)
        {
            var progress = list[i];
            var technology = state.FindTechnologyByName(progress.Name) ?? throw new InvalidOperationException("invalid technology progress");

            list[i] = progress with
            {
                // set budget to zero if max level
                CurrentResearchBudgetPercentage = (progress.CurrentLevel >= technology.MaxLevel) ? 0 : progress.CurrentResearchBudgetPercentage,
                // calculate new threshold
                ResourcesThreshold = (progress.CurrentLevel >= technology.MaxLevel) ? ResourceAmount.Empty : DetermineNewThreshold(technology, progress.CurrentLevel),
            };
        }

        // check how much budget needs re-adjustment
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
        }

        return state with
        {
            Progress = list,
        };
    }
}