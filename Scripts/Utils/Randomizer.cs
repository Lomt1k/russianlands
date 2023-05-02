using System;

namespace TextGameRPG.Scripts.Utils;

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

}
