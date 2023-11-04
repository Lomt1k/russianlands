using Newtonsoft.Json;
using SQLite;
using System;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
[Table("News")]
public class RawNewsData : RawDynamicData<NewsData>
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public DateTime date { get; set; }
    [MaxLength(64)]
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public int currentMailsCount { get; set; }
    public int totalMailsCount { get; set; }
    public string targetTelegramIds { get; set; } = "[]";

    public bool IsMailingCompleted()
    {
        return currentMailsCount >= totalMailsCount;
    }

    public float GetMailingProgress()
    {
        return (float)currentMailsCount / totalMailsCount;
    }

    public override void Fill(NewsData data)
    {
        id = data.id;
        date = data.date;
        title = data.title;
        description = data.description;
        currentMailsCount = data.currentMailsCount;
        totalMailsCount = data.totalMailsCount;
        targetTelegramIds = JsonConvert.SerializeObject(data.targetTelegramIds);
    }
}
