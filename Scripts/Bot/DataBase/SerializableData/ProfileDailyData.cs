using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Services.Mobs;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    public class ProfileDailyData : DataWithSession
    {
        // identification data
        [PrimaryKey]
        public long dbid { get; set; }
        public long telegram_id { get; set; }
        public string regDate { get; set; }
        [MaxLength(16)] public string regVersion { get; set; }
        [MaxLength(16)] public string lastVersion { get; set; }

        // for stats
        public int activityInSeconds { get; set; } = 1;

        // map mobs progress
        public MobDifficulty? locationMobsDifficulty { get; set; }
        public Dictionary<LocationType, List<byte>> defeatedLocationMobs { get; set; } = new();



        public static ProfileDailyData Create(ProfileData data, ProfileDynamicData dynamicData, ProfileBuildingsData buildingsData)
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

        public static ProfileDailyData Deserialize(RawProfileDailyData rawData)
        {
            return new ProfileDailyData
            {
                dbid = rawData.dbid,
                telegram_id = rawData.telegram_id,
                regDate = rawData.regDate,
                regVersion = rawData.regVersion,
                lastVersion = rawData.lastVersion,

                activityInSeconds = rawData.activityInSeconds,

                locationMobsDifficulty = rawData.locationMobsDifficulty,
                defeatedLocationMobs = JsonConvert.DeserializeObject<Dictionary<LocationType, List<byte>>>(rawData.defeatedLocationMobs),
            };
        }
    }
}
