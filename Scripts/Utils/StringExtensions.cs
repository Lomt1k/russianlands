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


    }
}
