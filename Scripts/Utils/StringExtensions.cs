using System;
using System.Collections;
using System.ComponentModel;
using System.Text;

public static class StringExtensions
{
    public static bool TryParse<T>(this string input, out T result)
    {
        result = default;
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                result = (T)converter.ConvertFromString(input);
                return true;
            }
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }

    public static bool IsCorrectNickname(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        for (int i = 0; i < input.Length; i++)
        {
            var symbol = input[i];
            if (char.IsLetterOrDigit(symbol))
                continue;

            //Пробелы могут быть только между словами (и не более одного пробела подряд)
            if (char.IsWhiteSpace(symbol))
            {
                var leftIndex = i - 1;
                var rightIndex = i + 1;
                if (leftIndex < 0 || rightIndex >= input.Length)
                    return false;

                if (char.IsWhiteSpace(input[leftIndex]) || char.IsWhiteSpace(input[rightIndex]))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Превращает число 1234567 в "1.2КК"
    /// </summary>
    public static string ShortView(this int input)
    {
        if (input < 1_000)
            return input.ToString();

        if (input < 1_000_000)
        {
            var rounded = (float)input / 1_000;
            var rest = rounded - (int)rounded;
            return rest < 0.1f ? $"{rounded:F0}K" : $"{rounded:F1}K";
        }

        if (input < 1_000_000_000)
        {
            var rounded = (float)input / 1_000_000;
            var rest = rounded - (int)rounded;
            return rest < 0.1f ? $"{rounded:F0}KK" : $"{rounded:F1}KK";
        }
        else
        {
            var rounded = (float)input / 1_000_000_000;
            var rest = rounded - (int)rounded;
            return rest < 0.1f ? $"{rounded:F0}KKK" : $"{rounded:F1}KKK";
        }
    }

    /// <summary>
    /// Превращает число 1234567 в "1,234,567"
    /// </summary>
    public static string View(this int input)
    {
        if (input < 1000 && input > -1000)
            return input.ToString();

        string positiveValue = Math.Abs(input).ToString();
        int separatorsCount = (positiveValue.Length - 1) / 3;
        int finalLength = positiveValue.Length + separatorsCount + (input < 0 ? 1 : 0);

        char[] result = new char[finalLength];
        int counter = 0;
        int charIndex = result.Length - 1;

        for (int i = positiveValue.Length - 1; i > 0; i--)
        {
            result[charIndex] = positiveValue[i];
            charIndex--;
            counter++;

            if (counter == 3)
            {
                result[charIndex] = ',';
                charIndex--;
                counter = 0;
            }
        }
        result[charIndex] = positiveValue[0];
        if (input < 0)
        {
            result[0] = '-';
        }

        return new string(result);
    }


}
