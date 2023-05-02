using System;
using System.Globalization;
using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

public static class DateTimeExtensions
{
    public static string GetView(this TimeSpan timeSpan, GameSession session, bool withCaption = false)
    {
        var sb = new StringBuilder();
        var spaceRequired = false;

        if (timeSpan.Days > 0)
        {
            sb.Append(timeSpan.Days + Localization.Get(session, "time_span_days"));
            spaceRequired = true;
        }
        if (timeSpan.Hours > 0)
        {
            AddSpaceIfRequired(sb);
            sb.Append(timeSpan.Hours + Localization.Get(session, "time_span_hours"));
            spaceRequired = true;
        }
        if (timeSpan.Minutes > 0)
        {
            AddSpaceIfRequired(sb);
            sb.Append(timeSpan.Minutes + Localization.Get(session, "time_span_minutes"));
            spaceRequired = true;
        }
        if (timeSpan.Seconds > 0 && timeSpan.Days < 1)
        {
            AddSpaceIfRequired(sb);
            sb.Append(timeSpan.Seconds + Localization.Get(session, "time_span_seconds"));
        }

        return withCaption
            ? Emojis.ElementClock + Localization.Get(session, "resource_name_time") + ' ' + sb.ToString()
            : Emojis.ElementClock + sb.ToString();

        void AddSpaceIfRequired(StringBuilder sb)
        {
            if (spaceRequired)
                sb.Append(' ');
        }
    }

    public static string GetShortView(this TimeSpan timeSpan, GameSession session)
    {
        string? result;
        if (timeSpan.TotalDays >= 1)
        {
            var days = (int)Math.Round(timeSpan.TotalDays);
            result = days + Localization.Get(session, "time_span_days");
        }
        else if (timeSpan.TotalHours >= 1)
        {
            var hours = (int)Math.Round(timeSpan.TotalHours);
            result = hours + Localization.Get(session, "time_span_hours");
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            var minutes = (int)Math.Round(timeSpan.TotalMinutes);
            result = minutes + Localization.Get(session, "time_span_minutes");
        }
        else
        {
            var seconds = (int)Math.Round(timeSpan.TotalSeconds);
            result = seconds + Localization.Get(session, "time_span_seconds");
        }

        return Emojis.ElementClock + result;
    }

    public static string AsDateTimeString(this DateTime dt)
    {
        return dt.ToString("dd.MM.yyyy H:mm:ss");
    }

    public static DateTime AsDateTime(this string str)
    {
        return DateTime.ParseExact(str, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture);
    }

    public static string AsDateString(this DateTime dt)
    {
        return dt.ToString("dd.MM.yyyy");
    }

    public static DateTime AsDate(this string str)
    {
        return DateTime.ParseExact(str, "dd.MM.yyyy", CultureInfo.InvariantCulture);
    }

}
