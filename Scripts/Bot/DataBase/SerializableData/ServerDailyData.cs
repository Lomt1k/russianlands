using SQLite;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    [Table("ServerDailyData")]
    public class ServerDailyData
    {
        [PrimaryKey]
        public string key { get; set; }
        public string value { get; set; }
    }
}
