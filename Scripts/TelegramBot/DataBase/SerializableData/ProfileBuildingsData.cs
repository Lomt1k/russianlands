using System.Data;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class ProfileBuildingsData : DatabaseSerializableData
    {
        public long dbid;

        public byte townHallLevel;
        public long townHallStartConstructionTime;

        // --- Хранилища
        public byte goldStorageLevel;
        public long goldStorageStartConstructionTime;
        public byte foodStorageLevel;
        public long foodStorageStartConstructionTime;
        public byte herbsStorageLevel;
        public long herbsStorageStartConstructionTime;
        public byte woodStorageLevel;
        public long woodStorageStartConstructionTime;


        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),

                new TableColumn("townHallLevel", "INTEGER", "1"),
                new TableColumn("townHallStartConstructionTime", "INTEGER", "0"),

                // --- Хранилища
                new TableColumn("goldStorageLevel", "INTEGER", "1"),
                new TableColumn("goldStorageStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodStorageLevel", "INTEGER", "0"),
                new TableColumn("foodStorageStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsStorageLevel", "INTEGER", "0"),
                new TableColumn("herbsStorageStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodStorageLevel", "INTEGER", "0"),
                new TableColumn("woodStorageStartConstructionTime", "INTEGER", "0"),
            };
        }

        public ProfileBuildingsData(DataRow data) : base(data)
        {
        }

        public async override Task<bool> UpdateInDatabase()
        {
            if (!isDeserializationCompleted)
                return true;

            var buildingsTable = TelegramBot.instance.dataBase[Table.ProfileBuildings] as ProfileBuildingsDataTable;
            var success = await buildingsTable.UpdateDataInDatabase(this);
            return success;
        }




    }
}
