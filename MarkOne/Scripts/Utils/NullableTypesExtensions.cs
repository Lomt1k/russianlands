using System;

public static class NullableTypesExtensions
{
    public static T EnsureNotNull<T>(this T? obj) where T : class
    {
        return obj ?? throw new ArgumentNullException(nameof(obj));
    }

    public static T EnsureNotNull<T>(this T? obj) where T : struct
    {
        return obj ?? throw new ArgumentNullException(nameof(obj));
    }
}
