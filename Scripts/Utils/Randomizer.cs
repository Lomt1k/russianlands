using System;

namespace TextGameRPG.Scripts.Utils
{
    public static class Randomizer
    {
        /// <summary>
        /// Возвращает результат - было ли успешным действие с вероятностью в процентах
        /// </summary>
        /// <param name="percent">вероятность успеха в %</param>
        /// <returns></returns>
        public static bool TryPercentage(int percent)
        {
            if (percent < 1)
                return false;
            if (percent > 99)
                return true;

            var result = new Random().Next(100);
            return result < percent;
        }

        /// <summary>
        /// Возвращает результат - было ли успешным действие с вероятностью в процентах
        /// </summary>
        /// <param name="percent">вероятность успеха в %</param>
        /// <returns></returns>
        public static bool TryPercentage(double percent)
        {
            if (percent < double.Epsilon)
                return false;
            if (percent >= 100)
                return true;

            var result = new Random().NextDouble();
            return result < percent / 100;
        }

        /// <summary>
        /// Возвращает число от 1 до 10
        /// - В 20% случаев вернет 1
        /// - В 17% случаев вернет 2
        /// - В 13% случаев вернет 3
        /// - В 13% случаев вернет 4
        /// - В 10% случаев вернет 5
        /// - В 7% случаев вернет 6
        /// - В 6% случаев вернет 7
        /// - В 6% случаев вернет 8
        /// - В 5% случаев вернет 9
        /// - В 3% случаев вернет 10
        /// </summary>
        /// <returns></returns>
        public static byte GetGrade()
        {
            var random = new Random();
            var grade = random.Next(100);
            if (grade < 20)
                return 1;
            if (grade < 37)
                return 2;
            if (grade < 50)
                return 3;
            if (grade < 63)
                return 4;
            if (grade < 73)
                return 5;
            if (grade < 80)
                return 6;
            if (grade < 86)
                return 7;
            if (grade < 92)
                return 8;
            if (grade < 97)
                return 9;

            return 10;
        }

    }
}
