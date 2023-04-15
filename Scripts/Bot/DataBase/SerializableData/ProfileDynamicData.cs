using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Potions;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    public class ProfileDynamicData : DataWithSession
    {
        public long dbid;
        public PlayerInventory inventory { get; } = new PlayerInventory();
        public List<PotionItem> potions { get; } = new List<PotionItem>();
        public PlayerQuestsProgress quests { get; } = new PlayerQuestsProgress();
        public List<ItemType> lastGeneratedItemTypes { get; } = new List<ItemType>();

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
    }
}
