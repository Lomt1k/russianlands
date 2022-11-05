using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class ProfileDynamicData : DatabaseSerializableData
    {
        static FieldInfo[] staticFieldsInfo = typeof(ProfileDynamicData).GetFields();
        public override FieldInfo[] fieldsInfo => staticFieldsInfo;

        public long dbid;
        public PlayerInventory inventory;
        public PlayerQuestsProgress quests;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("inventory", "TEXT", "'{ }'"),
                new TableColumn("quests", "TEXT", "'{ }'")
            };
        }

        public ProfileDynamicData(DataRow data) : base(data)
        {
        }

        protected override void Deserialize(DataRow data)
        {
            dbid = (long)data["dbid"];
            var inventoryJson = (string)data["inventory"];
            inventory = JsonConvert.DeserializeObject<PlayerInventory>(inventoryJson);
            var questsJson = (string)data["quests"];
            quests = JsonConvert.DeserializeObject<PlayerQuestsProgress>(questsJson);
        }

        public override async Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var dataTable = TelegramBot.instance.dataBase[Table.ProfilesDynamic] as ProfilesDynamicDataTable;
            var success = await dataTable.UpdateDataInDatabase(this);
            return success;
        }
    }
}
