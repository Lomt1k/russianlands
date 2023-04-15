using Newtonsoft.Json;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Potions;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using SQLite;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    [Table("ProfilesDynamic")]
    public class ProfileDynamicData : DataWithSession
    {
        private PlayerInventory? _inventory;
        private List<PotionItem>? _potions;
        private PlayerQuestsProgress? _quests;
        private List<ItemType>? _lastGeneratedItemTypes;

        [PrimaryKey]
        public long dbid { get; set; }
        [Column(nameof(inventory))]
        public string json_Inventory { get; set; } = "{}";
        [Column(nameof(potions))]
        public string json_potions { get; set; } = "[]";
        [Column(nameof(quests))]
        public string json_quests { get; set; } = "{}";
        [Column(nameof(lastGeneratedItemTypes))]
        public string json_lastGeneratedItemTypes { get; set; } = "[]";

        [Ignore]
        public PlayerInventory inventory => _inventory ??= JsonConvert.DeserializeObject<PlayerInventory>(json_Inventory);
        [Ignore]
        public List<PotionItem> potions => _potions ??= JsonConvert.DeserializeObject<List<PotionItem>>(json_potions);
        [Ignore]
        public PlayerQuestsProgress quests => _quests ??= JsonConvert.DeserializeObject<PlayerQuestsProgress>(json_quests);
        [Ignore]
        public List<ItemType> lastGeneratedItemTypes => _lastGeneratedItemTypes ??= JsonConvert.DeserializeObject<List<ItemType>>(json_lastGeneratedItemTypes);

        public void PrepareToSave()
        {
            json_Inventory = JsonConvert.SerializeObject(inventory);
            json_potions = JsonConvert.SerializeObject(potions);
            json_quests = JsonConvert.SerializeObject(quests);
            json_lastGeneratedItemTypes = JsonConvert.SerializeObject(lastGeneratedItemTypes);
        }

        public override void SetupSession(GameSession _session)
        {
            base.SetupSession(_session);
            inventory.SetupSession(_session);
        }
    }
}
