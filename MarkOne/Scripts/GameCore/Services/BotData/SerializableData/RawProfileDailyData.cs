using Newtonsoft.Json;
using SQLite;
using MarkOne.Scripts.GameCore.Services.Mobs;
using System;
using MarkOne.Scripts.GameCore.Quests;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("ProfileDailyData")]
public class RawProfileDailyData : RawDynamicData<ProfileDailyData>
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
    public byte townhallLevel { get; set; }
    public byte playerLevel { get; set; }
    public QuestId currentQuest { get; set; }
    public int currentQuestStage { get; set; }

    // map mobs progress
    public MobDifficulty? locationMobsDifficulty { get; set; }
    public string defeatedLocationMobs { get; set; } = "{}";

    public override void Fill(ProfileDailyData data)
    {
        dbid = data.dbid;
        telegram_id = data.telegram_id;
        regDate = data.regDate;
        regVersion = data.regVersion;
        regInfo = data.regInfo;
        lastVersion = data.lastVersion;

        activityInSeconds = data.activityInSeconds;
        battlesCount = data.battlesCount;
        townhallLevel = data.townhallLevel;
        playerLevel = data.playerLevel;
        currentQuest = data.currentQuest;
        currentQuestStage = data.currentQuestStage;

        locationMobsDifficulty = data.locationMobsDifficulty;
        defeatedLocationMobs = JsonConvert.SerializeObject(data.defeatedLocationMobs);
    }


}
