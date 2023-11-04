using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
public class NewsData
{
    public int id { get; set; }
    public DateTime date { get; set; }
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public int currentMailsCount { get; set; }
    public int totalMailsCount { get; set; }
    public List<long> targetTelegramIds { get; set; } = new();

    public bool IsMailingCompleted()
    {
        return currentMailsCount >= totalMailsCount;
    }

    public float GetMailingProgress()
    {
        return (float)currentMailsCount / totalMailsCount;
    }

    public static NewsData Deserialize(RawNewsData rawData)
    {
        return new NewsData()
        {
            id = rawData.id,
            date = rawData.date,
            title = rawData.title,
            description = rawData.description,
            currentMailsCount = rawData.currentMailsCount,
            totalMailsCount = rawData.totalMailsCount,
            targetTelegramIds = JsonConvert.DeserializeObject<List<long>>(rawData.targetTelegramIds),
        };
    }

    public static NewsData[] Deserialize(RawNewsData[] rawDatas)
    {
        var result = new NewsData[rawDatas.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = Deserialize(rawDatas[i]);
        }
        return result;
    }

}
