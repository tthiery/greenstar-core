using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Resources;

namespace GreenStar.Research;

public class TechnologyProgressEngine
{
    private readonly ITechnologyDefinitionLoader _technologyDefinitionStore;

    public TechnologyProgressEngine(ITechnologyDefinitionLoader technologyDefinitionStore)
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

        return state;
    }
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
        };

        return progress;
    }

    public static bool CanUserSeeTechnology(Technology technology, TechnologyProgress progress)
        => !technology.IsVisible
            && progress.IsDiscovered;
}
