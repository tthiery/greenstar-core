using System;

namespace GreenStar.Algorithms
{
    /// <summary>
    /// Algorithms required in relations to research
    /// </summary>
    public class ResearchAlgorithms
    {
        /// <summary>
        /// The percentage of research money to be invested in radical research
        /// </summary>
        public const double RadicalPercentage = 0.5;

        /// <summary>
        /// Calculate the amount of money, required to reach the next level of the provided technology
        /// </summary>
        /// <param name="technologyName"></param>
        /// <param name="currentLevel"></param>
        /// <returns></returns>
        public static int CalculateRequiredInvestmentForTechLevel(string technologyName, int currentLevel)
        {
            double targetMoney = 1000 * currentLevel;

            return Convert.ToInt32(targetMoney);
        }

        /// <summary>
        /// Convert a technology capability into stellar distance. Necessary to map speed and range
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static int ConvertTechnologyLevelToStellarDistance(int level)
        {
            return level * 100;
        }
    }
}
