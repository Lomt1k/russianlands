using SQLite;
using System;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("DailyReminders")]
public class DailyReminderData
{
    [PrimaryKey]
    public long dbid { get; set; }
    public long userId { get; set; }
    public DateTime timeToSend { get; set; }
    public LanguageCode languageCode { get; set; }
    public byte notificationIndex { get; set; }
    public byte lastLoginTownhall { get; set; }
    public bool isDailyBonusAvailable { get; set; }
}
