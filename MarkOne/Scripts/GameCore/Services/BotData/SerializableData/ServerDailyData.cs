using SQLite;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("ServerDailyData")]
public class ServerDailyData
{
    [PrimaryKey]
    public string key { get; set; }
    public string value { get; set; }
}
