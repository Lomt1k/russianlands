using Newtonsoft.Json;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Inventory;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Arena;

namespace MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

public class ProfileDynamicData : DataWithSession
{
    public long dbid { get; init; }
    public PlayerInventory inventory { get; init; } = new PlayerInventory();
    public List<PotionItem> potions { get; init; } = new List<PotionItem>();
    public PlayerQuestsProgress quests { get; init; } = new PlayerQuestsProgress();
    public List<ItemType> lastGeneratedItemTypes { get; init; } = new List<ItemType>();
    public PlayerArenaProgress? arenaProgress { get; init; }

    public ProfileDynamicData(long _dbid)
    {
        dbid = _dbid;
    }

    public override void SetupSession(GameSession _session)
    {
        base.SetupSession(_session);
        inventory.SetupSession(_session);
    }

    public static ProfileDynamicData Deserialize(RawProfileDynamicData rawData)
    {
        return new ProfileDynamicData(rawData.dbid)
        {
            inventory = JsonConvert.DeserializeObject<PlayerInventory>(rawData.inventory),
            potions = JsonConvert.DeserializeObject<List<PotionItem>>(rawData.potions),
            quests = JsonConvert.DeserializeObject<PlayerQuestsProgress>(rawData.quests),
            lastGeneratedItemTypes = JsonConvert.DeserializeObject<List<ItemType>>(rawData.lastGeneratedItemTypes),
            arenaProgress = rawData.arenaProgress != null ? JsonConvert.DeserializeObject<PlayerArenaProgress>(rawData.arenaProgress) : null,
        };
    }

    public bool HasArenaProgress()
    {
        return arenaProgress != null;
    }
}
