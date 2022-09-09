using System;

public static class DateTimeExtensions
{
    public static DateTime AsDateTime(this long longDateTime)
    {
        return new DateTime(longDateTime);
    }

    public static long AsLong(this DateTime dateTime)
    {
        return dateTime.Ticks;
    }
}
