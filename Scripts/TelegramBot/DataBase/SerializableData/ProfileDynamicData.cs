using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class ProfileDynamicData : DatabaseSerializableData
    {
        public long dbid;
        public InventoryBase inventory;

        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),
                new TableColumn("inventory", "TEXT", "'{ }'")
            };
        }

        public ProfileDynamicData(DataRow data) : base(data)
        {
        }

        protected override void Deserialize(DataRow data)
        {
            dbid = (long)data["dbid"];
            var inventoryJson = (string)data["inventory"];
            inventory = JsonConvert.DeserializeObject<InventoryBase>(inventoryJson);
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
