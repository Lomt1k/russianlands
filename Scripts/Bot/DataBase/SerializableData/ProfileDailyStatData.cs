using SQLite;
using System;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    [Table("ProfileDailyStatData")]
    public class ProfileDailyStatData
    {
        [PrimaryKey]
        public string statId { get; set; }
        public long dbid { get; set; }
        public long telegram_id { get; set; }
        public DateTime regDate { get; set; }
        [MaxLength(16)] public string regVersion { get; set; }
        [MaxLength(16)] public string lastVersion { get; set; }

        public int activityInSeconds { get; set; }
        public int daysAfterRegistration { get; set; }

        public static ProfileDailyStatData Create(ProfileDailyData data, string dateStr)
        {
            var now = DateTime.UtcNow;
            var regDate = data.regDate.AsDate();
            return new ProfileDailyStatData
            {
                statId = $"{dateStr} (dbid {data.dbid})",
                dbid = data.dbid,
                telegram_id = data.telegram_id,
                regDate = regDate,
                regVersion = data.regVersion,
                lastVersion = data.lastVersion,

                activityInSeconds = data.activityInSeconds,
                daysAfterRegistration = (now - regDate).Days,
            };
        }
    }
}
