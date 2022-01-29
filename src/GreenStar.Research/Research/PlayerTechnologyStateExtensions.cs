using System;
using System.Linq;

namespace GreenStar.Research;

public static class PlayerTechnologyStateExtensions
{
    public static bool HasAchievedTechnologyLevel(this PlayerTechnologyState self, string technologyName, int minimumLevel)
        => self.Progress.Any(p => p.Name == technologyName && p.CurrentLevel >= minimumLevel);

    public static PlayerTechnologyState WithProgress(this PlayerTechnologyState self, TechnologyProgress newItem)
    {
        var newProgress = self.Progress;

        if (newProgress.Any(p => p.Name == newItem.Name))
        {
            newProgress = newProgress.Select(p => (p.Name == newItem.Name) ? newItem : p).ToArray();
        }
        else
        {
            newProgress = new TechnologyProgress[self.Progress.Length + 1];
            Array.Copy(self.Progress, newProgress, self.Progress.Length);
            newProgress[self.Progress.Length] = newItem;
        }

        return self with
        {
            Progress = newProgress,
        };
    }
}
