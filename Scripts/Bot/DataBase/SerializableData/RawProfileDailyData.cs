using Newtonsoft.Json;
using SQLite;
using TextGameRPG.Scripts.GameCore.Services.Mobs;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData;

[Table("ProfileDailyData")]
public class RawProfileDailyData : RawDynamicData<ProfileDailyData>
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
    public string defeatedLocationMobs { get; set; } = "{}";

    public override void Fill(ProfileDailyData data)
    {
        dbid = data.dbid;
        telegram_id = data.telegram_id;
        regDate = data.regDate;
        regVersion = data.regVersion;
        lastVersion = data.lastVersion;

        activityInSeconds = data.activityInSeconds;

        locationMobsDifficulty = data.locationMobsDifficulty;
        defeatedLocationMobs = JsonConvert.SerializeObject(data.defeatedLocationMobs);
    }


}
