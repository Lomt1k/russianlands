using Newtonsoft.Json;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Inventory;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

public class ProfileDynamicData : DataWithSession
{
    public long dbid { get; }
    public PlayerInventory inventory { get; } = new PlayerInventory();
    public List<PotionItem> potions { get; } = new List<PotionItem>();
    public PlayerQuestsProgress quests { get; } = new PlayerQuestsProgress();
    public List<ItemType> lastGeneratedItemTypes { get; } = new List<ItemType>();

    public ProfileDynamicData(long _dbid)
    {
        dbid = _dbid;
    }

    public ProfileDynamicData(long _dbid, PlayerInventory _inventory, List<PotionItem> _potions, PlayerQuestsProgress _quests, List<ItemType> _lastGeneratedItemTypes)
    {
        dbid = _dbid;
        inventory = _inventory;
        potions = _potions;
        quests = _quests;
        lastGeneratedItemTypes = _lastGeneratedItemTypes;
    }

    public override void SetupSession(GameSession _session)
    {
        base.SetupSession(_session);
        inventory.SetupSession(_session);
    }

    public static ProfileDynamicData Deserialize(RawProfileDynamicData rawData)
    {
        return new ProfileDynamicData(rawData.dbid,
            JsonConvert.DeserializeObject<PlayerInventory>(rawData.inventory),
            JsonConvert.DeserializeObject<List<PotionItem>>(rawData.potions),
            JsonConvert.DeserializeObject<PlayerQuestsProgress>(rawData.quests),
            JsonConvert.DeserializeObject<List<ItemType>>(rawData.lastGeneratedItemTypes));
    }
}
