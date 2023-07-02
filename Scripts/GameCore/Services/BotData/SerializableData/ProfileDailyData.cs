using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Services.Mobs;
using System;
using MarkOne.Scripts.GameCore.Quests;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

public class ProfileDailyData : DataWithSession
{
    // identification data
    [PrimaryKey]
    public long dbid { get; set; }
    public long telegram_id { get; set; }
    public DateTime regDate { get; set; }
    [MaxLength(16)] public string regVersion { get; set; } = string.Empty;
    [MaxLength(32)] public string regInfo { get; set; } = string.Empty;
    [MaxLength(16)] public string lastVersion { get; set; } = string.Empty;

    // for stats
    public int activityInSeconds { get; set; } = 1;
    public ushort battlesCount { get; set; }
    public byte townhallLevel => session.profile.buildingsData.townHallLevel;
    public byte playerLevel => session.profile.data.level;
    public QuestId currentQuest => session.profile.dynamicData.quests.GetFocusedQuest() ?? QuestId.None;
    public int currentQuestStage => session.profile.dynamicData.quests.GetStage(session.profile.dynamicData.quests.GetFocusedQuest() ?? QuestId.None);

    // game data
    public MobDifficulty? locationMobsDifficulty { get; set; }
    public Dictionary<LocationId, List<byte>> defeatedLocationMobs { get; set; } = new();
    public int lastCrossroadId { get; set; }



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
            regInfo = data.regInfo,
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
            regInfo = rawData.regInfo,
            lastVersion = rawData.lastVersion,

            activityInSeconds = rawData.activityInSeconds,
            battlesCount = rawData.battlesCount,

            locationMobsDifficulty = rawData.locationMobsDifficulty,
            defeatedLocationMobs = JsonConvert.DeserializeObject<Dictionary<LocationId, List<byte>>>(rawData.defeatedLocationMobs),
        };
    }
}
