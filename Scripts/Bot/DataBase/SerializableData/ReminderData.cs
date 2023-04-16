using SQLite;
using System;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    [Table("Reminders")]
    public class ReminderData
    {
        [PrimaryKey]
        public long dbid { get; set; }
        public long userId { get; set; }
        public DateTime timeToSend { get; set; }
        public LanguageCode languageCode { get; set; }
    }
}
