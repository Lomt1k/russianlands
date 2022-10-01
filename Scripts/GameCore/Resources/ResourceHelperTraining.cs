using System;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public static partial class ResourceHelper
    {
        private static Dictionary<int, int> boostTrainingInDiamondsBySeconds = new Dictionary<int, int>
        {
            { 60, 1 },
            { 18_000, 45 },
            { 86_400, 250 },
            { 604_800, 1_000 },
        };

        private static KeyValuePair<int, int> minBoostTrainingInDiamondsBySeconds;
        private static KeyValuePair<int, int> maxBoostTrainingInDiamondsBySeconds;

        public static int CalculateTrainingBoostPriceInDiamonds(int seconds)
        {
            if (seconds <= minBoostTrainingInDiamondsBySeconds.Key)
                return minBoostTrainingInDiamondsBySeconds.Value;

            if (seconds >= maxBoostTrainingInDiamondsBySeconds.Key)
            {
                var mult = (float)seconds / maxBoostTrainingInDiamondsBySeconds.Key;
                return (int)(maxBoostTrainingInDiamondsBySeconds.Value * mult);
            }

            var lowerKVP = new KeyValuePair<int, int>();
            var upperKVP = new KeyValuePair<int, int>();
            foreach (var kvp in boostTrainingInDiamondsBySeconds)
            {
                if (seconds < kvp.Key)
                {
                    upperKVP = kvp;
                    break;
                }
                lowerKVP = kvp;
            }

            var secondsDelta = upperKVP.Key - lowerKVP.Key;
            var priceDelta = upperKVP.Value - lowerKVP.Value;
            var progression = (float)(seconds - lowerKVP.Key) / secondsDelta;

            return (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
        }


        private static Dictionary<int, int> defaultResourceTrainingTimeByCurrentLevel = new Dictionary<int, int>
        {
            { 1, 3600 },
            { 2, 7200 },
            { 3, 14_400 },
            { 4, 21_600 },
            { 5, 32_400 },
            { 6, 43_200 },
            { 7, 54_000 },
            { 8, 64_800 },
            { 9, 75_600 },
            { 10, 86_400 },
            { 11, 97_200 },
            { 12, 108_000 },
            { 13, 118_800 },
            { 14, 129_600 },
            { 15, 140_400 },
            { 16, 151_200 },
            { 17, 162_000 },
            { 18, 172_800 },
        };

        public static int GetDefaultResourceTrainingTimeInSeconds(int currentLevel)
        {
            if (defaultResourceTrainingTimeByCurrentLevel.TryGetValue(currentLevel, out var timeInSeconds))
            {
                return timeInSeconds;
            }
            return 172_800;
        }

        private static Dictionary<int, int> woodTrainingTimeByCurrentLevel = new Dictionary<int, int>
        {
            { 1, 7200 },
            { 2, 14_400 },
            { 3, 28_800 },
            { 4, 43_200 },
            { 5, 57_600 },
            { 6, 72_000 },
            { 7, 86_400 },
            { 8, 115_200 },
            { 9, 144_000 },
            { 10, 172_800 },
        };

        public static int GetWoodTrainingTimeInSeconds(int currentLevel)
        {
            if (woodTrainingTimeByCurrentLevel.TryGetValue(currentLevel, out var timeInSeconds))
            {
                return timeInSeconds;
            }
            return 172_800;
        }

        private static Dictionary<int, int> warriorTrainingTimeByCurrentLevel = new Dictionary<int, int>
        {
            { 1, 60 },
            { 2, 60 },
            { 3, 60 },
            { 4, 240 },
            { 5, 420 },
            { 6, 600 },
            { 7, 1_200 },
            { 8, 1_800 },
            { 9, 2_400 },
            { 10, 3_600 },
            { 11, 5_400 },
            { 12, 7_200 },
            { 13, 9_000 },
            { 14, 10_800 },
            { 15, 14_400 },
            { 16, 19_800 },
            { 17, 25_200 },
            { 18, 30_600 },
            { 19, 36_000 },
            { 20, 38_700 },
            { 21, 41_400 },
            { 22, 44_100 },
            { 23, 46_800 },
            { 24, 49_500 },
            { 25, 52_200 },
            { 26, 54_900 },
            { 27, 57_600 },
            { 28, 61_200 },
            { 29, 64_800 },
            { 30, 68_400 },
            { 31, 72_900 },
            { 32, 77_400 },
            { 33, 81_900 },
            { 34, 86_400 },
            { 35, 90_900 },
            { 36, 95_400 },
            { 37, 100_800 },
            { 38, 108_000 },
            { 39, 115_200 },
            { 40, 129_600 },
            { 41, 144_000 },
            { 42, 165_600 },
            { 43, 187_200 },
            { 44, 244_800 },
            { 45, 259_200 },
        };

        public static int GetWarriorTrainingTimeInSeconds(int currentLevel)
        {
            if (warriorTrainingTimeByCurrentLevel.TryGetValue(currentLevel, out var timeInSeconds))
            {
                return timeInSeconds;
            }
            return 259_200;
        }

    }
}
