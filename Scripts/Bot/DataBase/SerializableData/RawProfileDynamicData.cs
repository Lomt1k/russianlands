using Newtonsoft.Json;
using SQLite;

namespace MarkOne.Scripts.Bot.DataBase.SerializableData;

[Table("ProfilesDynamic")]
public class RawProfileDynamicData : RawDynamicData<ProfileDynamicData>
{
    [PrimaryKey]
    public long dbid { get; set; }
    public string inventory { get; set; } = "{}";
    public string potions { get; set; } = "[]";
    public string quests { get; set; } = "{}";
    public string lastGeneratedItemTypes { get; set; } = "[]";

    public override void Fill(ProfileDynamicData data)
    {
        dbid = data.dbid;
        inventory = JsonConvert.SerializeObject(data.inventory);
        potions = JsonConvert.SerializeObject(data.potions);
        quests = JsonConvert.SerializeObject(data.quests);
        lastGeneratedItemTypes = JsonConvert.SerializeObject(data.lastGeneratedItemTypes);
    }

}
