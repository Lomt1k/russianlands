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

        public static ResourceData CalculateTrainingBoostPriceInDiamonds(int seconds)
        {
            var amount = 0;
            if (seconds <= minBoostTrainingInDiamondsBySeconds.Key)
            {
                amount = minBoostTrainingInDiamondsBySeconds.Value;
                return new ResourceData(ResourceId.Diamond, amount);
            }

            if (seconds >= maxBoostTrainingInDiamondsBySeconds.Key)
            {
                var mult = (float)seconds / maxBoostTrainingInDiamondsBySeconds.Key;
                amount = (int)(maxBoostTrainingInDiamondsBySeconds.Value * mult);
                return new ResourceData(ResourceId.Diamond, amount);
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

            amount = (int)Math.Round(progression * priceDelta) + lowerKVP.Value;
            return new ResourceData(ResourceId.Diamond, amount);
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
            // Ратуша 2
            { 1, 30 },
            { 2, 30 },
            { 3, 30 },
            // Ратуша 3
            { 4, 240 },
            { 5, 420 },
            { 6, 600 },
            // Ратуша 4
            { 7, 1_800 },
            { 8, 3_600 },
            { 9, 5_400 },
            // Ратуша 5
            { 10, 10_800 },
            { 11, 16_200 },
            { 12, 21_600 },
            { 13, 27_000 },
            { 14, 32_400 },
            // Ратуша 6
            { 15, 43_200 },
            { 16, 56_600 },
            { 17, 72_000 },
            { 18, 86_400 },
            { 19, 108_000 },
            // Ратуша 7
            { 20, 151_200 },
            { 21, 165_600 },
            { 22, 172_800 },
            { 23, 187_200 },
            { 24, 198_000 },
            { 25, 208_800 },
            { 26, 219_600 },
            { 27, 230_400 },
            // Ратуша 8
            { 28, 244_800 },
            { 29, 259_200 },
            { 30, 273_600 },
            { 31, 291_600 },
            { 32, 309_600 },
            { 33, 327_600 },
            { 34, 345_600 },
            // Ратуша 9 (?)
            { 35, 363_600 },
            { 36, 381_600 },
            { 37, 403_200 },
            { 38, 432_000 },
            { 39, 460_800 },
            { 40, 518_400 },
            { 41, 561_600 },
            // Ратуша 10 (?)
            { 42, 604_800 },
            { 43, 691_200 },
            { 44, 777_600 },
            { 45, 864_000 },
        };

        public static int GetWarriorTrainingTimeInSeconds(int currentLevel)
        {
            if (warriorTrainingTimeByCurrentLevel.TryGetValue(currentLevel, out var timeInSeconds))
            {
                return timeInSeconds;
            }
            return 864_000;
        }

    }
}
