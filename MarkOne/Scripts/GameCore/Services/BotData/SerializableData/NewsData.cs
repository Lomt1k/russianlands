using SQLite;
using System;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
[Table("News")]
public class NewsData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public DateTime date { get; set; }
    [MaxLength(64)]
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
}
