using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.Bot.DataBase.TablesStructure;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Potions;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    public class ProfileDynamicData : DatabaseSerializableData
    {
        static FieldInfo[] staticFieldsInfo = typeof(ProfileDynamicData).GetFields();
        public override FieldInfo[] fieldsInfo => staticFieldsInfo;

        public long dbid;
        public PlayerInventory inventory;
        public List<PotionItem> potions;
        public PlayerQuestsProgress quests;
        public List<ItemType> lastGeneratedItemTypes;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("inventory", "TEXT", "'{}'"),
                new TableColumn("potions", "TEXT", "'[]'"),
                new TableColumn("quests", "TEXT", "'{}'"),
                new TableColumn("lastGeneratedItemTypes", "TEXT", "'[]'")
            };
        }

        public ProfileDynamicData(DataRow data) : base(data)
        {
        }

        protected override void Deserialize(DataRow data)
        {
            dbid = (long)data["dbid"];
            var json = (string)data[nameof(inventory)];
            inventory = JsonConvert.DeserializeObject<PlayerInventory>(json);
            json = (string)data[nameof(potions)];
            potions = JsonConvert.DeserializeObject<List<PotionItem>>(json);
            json = (string)data[nameof(quests)];
            quests = JsonConvert.DeserializeObject<PlayerQuestsProgress>(json);
            json = (string)data[nameof(lastGeneratedItemTypes)];
            lastGeneratedItemTypes = JsonConvert.DeserializeObject<List<ItemType>>(json);
        }

        public override async Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var dataTable = BotController.dataBase[Table.ProfilesDynamic] as ProfilesDynamicDataTable;
            var success = await dataTable.UpdateDataInDatabase(this).FastAwait();
            return success;
        }

        public override void SetupSession(GameSession _session)
        {
            base.SetupSession(_session);
            inventory.SetupSession(_session);
        }
    }
}
