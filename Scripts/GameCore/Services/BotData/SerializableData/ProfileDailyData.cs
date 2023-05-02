using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Services.Mobs;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

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
    public Dictionary<LocationId, List<byte>> defeatedLocationMobs { get; set; } = new();



    public MobDifficulty GetLocationMobDifficulty()
    {
        return locationMobsDifficulty ??= MobDifficultyCalculator.GetActualDifficultyForPlayer(session.player);
    }

    public List<byte> GetLocationDefeatedMobs(LocationId locationId)
    {
        if (defeatedLocationMobs.TryGetValue(locationId, out var result))
        {
            return result;
        }
        var newList = new List<byte>();
        defeatedLocationMobs.Add(locationId, newList);
        return newList;
    }

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
            defeatedLocationMobs = JsonConvert.DeserializeObject<Dictionary<LocationId, List<byte>>>(rawData.defeatedLocationMobs),
        };
    }
}
