using SQLite;

namespace TextGameRPG.Scripts.GameCore.Services
{
    [Table("ServerDailyData")]
    public class ServerDailyData
    {
        [PrimaryKey]
        public string key { get; set; }
        public string value { get; set; }
    }
}
