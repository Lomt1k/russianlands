using System;

namespace TextGameRPG.Scripts.Utils
{
    public static class Randomizer
    {
        public static Random random = new Random();

        public static bool TryPercentage(int percent)
        {
            if (percent < 1)
                return false;
            if (percent > 99)
                return true;

            var result = random.Next(100);
            return result < percent;
        }

        public static bool TryPercentage(double percent)
        {
            if (percent < double.Epsilon)
                return false;
            if (percent >= 100)
                return true;

            var result = random.NextDouble();
            return result < percent / 100;
        }

    }
}
