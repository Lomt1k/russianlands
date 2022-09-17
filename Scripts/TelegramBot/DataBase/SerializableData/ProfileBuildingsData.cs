using System.Data;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class ProfileBuildingsData : DatabaseSerializableData
    {
        public long dbid;

        public byte townHallLevel = 1;
        public long townHallStartConstructionTime;
        public byte hospitalLevel = 1;
        public long hospitalStartConstructionTime;

        // --- Хранилища
        public byte goldStorageLevel = 1;
        public long goldStorageStartConstructionTime;
        public byte foodStorageLevel;
        public long foodStorageStartConstructionTime;
        public byte herbsStorageLevel;
        public long herbsStorageStartConstructionTime;
        public byte woodStorageLevel;
        public long woodStorageStartConstructionTime;

        // --- Добыча ресурсов: Золото
        public byte goldProdFirstLevel = 1;
        public long goldProdFirstStartConstructionTime;
        public long goldProdFirstStartFarmTime;
        public int goldProdFirstWorkerFirst;
        public int goldProdFirstWorkerSecond;

        public byte goldProdSecondLevel;
        public long goldProdSecondStartConstructionTime;
        public long goldProdSecondStartFarmTime;
        public int goldProdSecondWorkerFirst;
        public int goldProdSecondWorkerSecond;

        public byte goldProdThirdLevel;
        public long goldProdThirdStartConstructionTime;
        public long goldProdThirdStartFarmTime;
        public int goldProdThirdWorkerFirst;
        public int goldProdThirdWorkerSecond;

        // --- Добыча ресурсов: Еда
        public byte foodProdFirstLevel;
        public long foodProdFirstStartConstructionTime;
        public long foodProdFirstStartFarmTime;
        public int foodProdFirstWorkerFirst;
        public int foodProdFirstWorkerSecond;

        public byte foodProdSecondLevel;
        public long foodProdSecondStartConstructionTime;
        public long foodProdSecondStartFarmTime;
        public int foodProdSecondWorkerFirst;
        public int foodProdSecondWorkerSecond;

        public byte foodProdThirdLevel;
        public long foodProdThirdStartConstructionTime;
        public long foodProdThirdStartFarmTime;
        public int foodProdThirdWorkerFirst;
        public int foodProdThirdWorkerSecond;

        // --- Добыча ресурсов: Травы
        public byte herbsProdFirstLevel;
        public long herbsProdFirstStartConstructionTime;
        public long herbsProdFirstStartFarmTime;
        public int herbsProdFirstWorkerFirst;
        public int herbsProdFirstWorkerSecond;

        public byte herbsProdSecondLevel;
        public long herbsProdSecondStartConstructionTime;
        public long herbsProdSecondStartFarmTime;
        public int herbsProdSecondWorkerFirst;
        public int herbsProdSecondWorkerSecond;

        public byte herbsProdThirdLevel;
        public long herbsProdThirdStartConstructionTime;
        public long herbsProdThirdStartFarmTime;
        public int herbsProdThirdWorkerFirst;
        public int herbsProdThirdWorkerSecond;

        // --- Добыча ресурсов: Дерево
        public byte woodProdFirstLevel;
        public long woodProdFirstStartConstructionTime;
        public long woodProdFirstStartFarmTime;
        public int woodProdFirstWorkerFirst;
        public int woodProdFirstWorkerSecond;

        public byte woodProdSecondLevel;
        public long woodProdSecondStartConstructionTime;
        public long woodProdSecondStartFarmTime;
        public int woodProdSecondWorkerFirst;
        public int woodProdSecondWorkerSecond;


        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),

                new TableColumn("townHallLevel", "INTEGER", "1"),
                new TableColumn("townHallStartConstructionTime", "INTEGER", "0"),
                new TableColumn("hospitalLevel", "INTEGER", "1"),
                new TableColumn("hospitalStartConstructionTime", "INTEGER", "0"),

                // --- Хранилища
                new TableColumn("goldStorageLevel", "INTEGER", "1"),
                new TableColumn("goldStorageStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodStorageLevel", "INTEGER", "0"),
                new TableColumn("foodStorageStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsStorageLevel", "INTEGER", "0"),
                new TableColumn("herbsStorageStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodStorageLevel", "INTEGER", "0"),
                new TableColumn("woodStorageStartConstructionTime", "INTEGER", "0"),

                // --- Добыча ресурсов: Золото
                new TableColumn("goldProdFirstLevel", "INTEGER", "1"),
                new TableColumn("goldProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("goldProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("goldProdFirstWorkerFirst", "INTEGER", "0"),
                new TableColumn("goldProdFirstWorkerSecond", "INTEGER", "0"),

                new TableColumn("goldProdSecondLevel", "INTEGER", "0"),
                new TableColumn("goldProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("goldProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("goldProdSecondWorkerFirst", "INTEGER", "0"),
                new TableColumn("goldProdSecondWorkerSecond", "INTEGER", "0"),

                new TableColumn("goldProdThirdLevel", "INTEGER", "0"),
                new TableColumn("goldProdThirdStartConstructionTime", "INTEGER", "0"),
                new TableColumn("goldProdThirdStartFarmTime", "INTEGER", "0"),
                new TableColumn("goldProdThirdWorkerFirst", "INTEGER", "0"),
                new TableColumn("goldProdThirdWorkerSecond", "INTEGER", "0"),

                // --- Добыча ресурсов: Еда
                new TableColumn("foodProdFirstLevel", "INTEGER", "0"),
                new TableColumn("foodProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("foodProdFirstWorkerFirst", "INTEGER", "0"),
                new TableColumn("foodProdFirstWorkerSecond", "INTEGER", "0"),

                new TableColumn("foodProdSecondLevel", "INTEGER", "0"),
                new TableColumn("foodProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("foodProdSecondWorkerFirst", "INTEGER", "0"),
                new TableColumn("foodProdSecondWorkerSecond", "INTEGER", "0"),

                new TableColumn("foodProdThirdLevel", "INTEGER", "0"),
                new TableColumn("foodProdThirdStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodProdThirdStartFarmTime", "INTEGER", "0"),
                new TableColumn("foodProdThirdWorkerFirst", "INTEGER", "0"),
                new TableColumn("foodProdThirdWorkerSecond", "INTEGER", "0"),

                // --- Добыча ресурсов: Травы
                new TableColumn("herbsProdFirstLevel", "INTEGER", "0"),
                new TableColumn("herbsProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("herbsProdFirstWorkerFirst", "INTEGER", "0"),
                new TableColumn("herbsProdFirstWorkerSecond", "INTEGER", "0"),

                new TableColumn("herbsProdSecondLevel", "INTEGER", "0"),
                new TableColumn("herbsProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("herbsProdSecondWorkerFirst", "INTEGER", "0"),
                new TableColumn("herbsProdSecondWorkerSecond", "INTEGER", "0"),

                new TableColumn("herbsProdThirdLevel", "INTEGER", "0"),
                new TableColumn("herbsProdThirdStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsProdThirdStartFarmTime", "INTEGER", "0"),
                new TableColumn("herbsProdThirdWorkerFirst", "INTEGER", "0"),
                new TableColumn("herbsProdThirdWorkerSecond", "INTEGER", "0"),

                // --- Добыча ресурсов: Дерево
                new TableColumn("woodProdFirstLevel", "INTEGER", "0"),
                new TableColumn("woodProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("woodProdFirstWorkerFirst", "INTEGER", "0"),
                new TableColumn("woodProdFirstWorkerSecond", "INTEGER", "0"),

                new TableColumn("woodProdSecondLevel", "INTEGER", "0"),
                new TableColumn("woodProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("woodProdSecondWorkerFirst", "INTEGER", "0"),
                new TableColumn("woodProdSecondWorkerSecond", "INTEGER", "0"),
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
