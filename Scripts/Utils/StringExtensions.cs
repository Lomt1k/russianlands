using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

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

        if (input.Length < 3 || input.Length > 16)
            return false;

        for (int i = 0; i < input.Length; i++)
        {
            var symbol = input[i];
            if (!char.IsLetterOrDigit(symbol) && !char.IsWhiteSpace(symbol))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Превращает число 1234567 в "1.2M"
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
            return rest < 0.1f ? $"{rounded:F0}M" : $"{rounded:F1}M";
        }
        else
        {
            var rounded = (float)input / 1_000_000_000;
            var rest = rounded - (int)rounded;
            return rest < 0.1f ? $"{rounded:F0}B" : $"{rounded:F1}B";
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

    /// <summary>
    /// Удаляет HTML все теги
    /// </summary>
    public static string RemoveHtmlTags(this string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    /// <summary>
    /// Оборачивает текст в html тэг <b> (жирный)
    /// </summary>
    public static string Bold(this string input)
    {
        return "<b>" + input + "</b>";
    }

    /// <summary>
    /// Оборачивает текст в html тэг <i> (курсив)
    /// </summary>
    public static string Italic(this string input)
    {
        return "<i>" + input + "</i>";
    }

    /// <summary>
    /// Оборачивает текст в html тэг <u> (подчеркнутый текст)
    /// </summary>
    public static string Underline(this string input)
    {
        return "<u>" + input + "</u>";
    }

    /// <summary>
    /// Оборачивает текст в html тэг <s> (зачеркнутый текст)
    /// </summary>
    public static string Strike(this string input)
    {
        return "<s>" + input + "</s>";
    }

    /// <summary>
    /// Оборачивает текст в html тэг <code> (код)
    /// </summary>
    public static string CodeBlock(this string input)
    {
        return "<code>" + input + "</code>";
    }

    /// <summary>
    /// Оборачивает текст в html тэг <pre> (форматированный текст)
    /// </summary>
    public static string Preformatted(this string input)
    {
        return "<pre>" + input + "</pre>";
    }


}
