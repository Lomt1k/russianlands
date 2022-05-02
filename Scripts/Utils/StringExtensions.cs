using System;
using System.ComponentModel;

namespace TextGameRPG.Scripts.Utils
{
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


    }
}
