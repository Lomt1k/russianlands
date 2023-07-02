using MarkOne.Scripts.GameCore.Quests;
using SQLite;
using System;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("ProfileDailyStatData")]
public class ProfileDailyStatData
{
    [PrimaryKey]
    public string statId { get; set; } = string.Empty;
    public long dbid { get; set; }
    public long telegram_id { get; set; }
    public DateTime regDate { get; set; }
    [MaxLength(16)] public string regVersion { get; set; } = string.Empty;
    [MaxLength(32)] public string regInfo { get; set; } = string.Empty;
    [MaxLength(16)] public string lastVersion { get; set; } = string.Empty;

    public int activityInSeconds { get; set; }
    public int daysAfterRegistration { get; set; }
    public ushort battlesCount { get; set; }
    public byte townhallLevel { get; set; }
    public byte playerLevel { get; set; }
    public QuestId currentQuest { get; set; }
    public int currentQuestStage { get; set; }

    public static ProfileDailyStatData Create(RawProfileDailyData data, DateTime date, string dateStr)
    {
        var regDate = data.regDate;
        return new ProfileDailyStatData
        {
            statId = $"{dateStr} | dbid {data.dbid}",
            dbid = data.dbid,
            telegram_id = data.telegram_id,
            regDate = regDate,
            regVersion = data.regVersion,
            regInfo = data.regInfo,
            lastVersion = data.lastVersion,

            activityInSeconds = data.activityInSeconds,
            daysAfterRegistration = (date - regDate.Date).Days,
            battlesCount = data.battlesCount,
            townhallLevel = data.townhallLevel,
            playerLevel = data.playerLevel,
            currentQuest = data.currentQuest,
            currentQuestStage = data.currentQuestStage,
        };
    }
}
