using SQLite;
using System;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.Bot.DataBase.SerializableData;

[Table("DailyReminders")]
public class DailyReminderData
{
    [PrimaryKey]
    public long dbid { get; set; }
    public long userId { get; set; }
    public DateTime timeToSend { get; set; }
    public LanguageCode languageCode { get; set; }
}
