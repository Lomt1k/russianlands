using SQLite;
using System;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    [Table("ProfileDailyData")]
    public class ProfileDailyData : DataWithSession
    {
        // identification data
        [PrimaryKey]
        public long dbid { get; set; }
        public long telegram_id { get; set; }
        public string regDate { get; set; }
        [MaxLength(16)] public string regVersion { get; set; }
        [MaxLength(16)] public string lastVersion { get; set; }

        //for stats
        public int activityInSeconds { get; set; } = 1;


        public static ProfileDailyData Create(ProfileData data)
        {
            return new ProfileDailyData
            {
                dbid = data.dbid,
                telegram_id = data.telegram_id,
                regDate = data.regDate,
                regVersion = data.regVersion,
                lastVersion = data.lastVersion,
            };
        }
    }
}
