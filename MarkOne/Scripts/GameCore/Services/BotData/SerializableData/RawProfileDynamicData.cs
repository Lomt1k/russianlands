using Newtonsoft.Json;
using SQLite;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

[Table("ProfilesDynamic")]
public class RawProfileDynamicData : RawDynamicData<ProfileDynamicData>
{
    [PrimaryKey]
    public long dbid { get; set; }
    public string inventory { get; set; } = "{}";
    public string potions { get; set; } = "[]";
    public string quests { get; set; } = "{}";
    public string lastGeneratedItemTypes { get; set; } = "[]";
    public string? arenaProgress { get; set; } = null;
    public string offers { get; set; } = "[]";
    public string avatars { get; set; } = "[]";

    public override void Fill(ProfileDynamicData data)
    {
        dbid = data.dbid;
        inventory = JsonConvert.SerializeObject(data.inventory);
        potions = JsonConvert.SerializeObject(data.potions);
        quests = JsonConvert.SerializeObject(data.quests);
        lastGeneratedItemTypes = JsonConvert.SerializeObject(data.lastGeneratedItemTypes);
        arenaProgress = JsonConvert.SerializeObject(data.arenaProgress);
        offers = JsonConvert.SerializeObject(data.offers);
        avatars = JsonConvert.SerializeObject(data.avatars);
    }

}
