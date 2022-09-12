using System;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

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

    public static string GetView(this TimeSpan timeSpan, GameSession session)
    {
        var sb = new StringBuilder();
        bool spaceRequired = false;

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

        return sb.ToString();

        void AddSpaceIfRequired(StringBuilder sb)
        {
            if (spaceRequired)
                sb.Append(' ');
        }
    }
}
