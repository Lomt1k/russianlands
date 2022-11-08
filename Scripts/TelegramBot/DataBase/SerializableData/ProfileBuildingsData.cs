using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.DataBase.TablesStructure;

namespace TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData
{
    public class ProfileBuildingsData : DatabaseSerializableData
    {
        static FieldInfo[] staticFieldsInfo = typeof(ProfileBuildingsData).GetFields();
        public override FieldInfo[] fieldsInfo => staticFieldsInfo;

        public long dbid;

        public byte townHallLevel = 1;
        public long townHallStartConstructionTime;
        public byte tyrLevel = 1;
        public long tyrConstructionTime;
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
        public byte goldProdFirstWorkerFirst;
        public byte goldProdFirstWorkerSecond;

        public byte goldProdSecondLevel;
        public long goldProdSecondStartConstructionTime;
        public long goldProdSecondStartFarmTime;
        public byte goldProdSecondWorkerFirst;
        public byte goldProdSecondWorkerSecond;

        public byte goldProdThirdLevel;
        public long goldProdThirdStartConstructionTime;
        public long goldProdThirdStartFarmTime;
        public byte goldProdThirdWorkerFirst;
        public byte goldProdThirdWorkerSecond;

        // --- Добыча ресурсов: Еда
        public byte foodProdFirstLevel;
        public long foodProdFirstStartConstructionTime;
        public long foodProdFirstStartFarmTime;
        public byte foodProdFirstWorkerFirst;
        public byte foodProdFirstWorkerSecond;

        public byte foodProdSecondLevel;
        public long foodProdSecondStartConstructionTime;
        public long foodProdSecondStartFarmTime;
        public byte foodProdSecondWorkerFirst;
        public byte foodProdSecondWorkerSecond;

        public byte foodProdThirdLevel;
        public long foodProdThirdStartConstructionTime;
        public long foodProdThirdStartFarmTime;
        public byte foodProdThirdWorkerFirst;
        public byte foodProdThirdWorkerSecond;

        // --- Добыча ресурсов: Травы
        public byte herbsProdFirstLevel;
        public long herbsProdFirstStartConstructionTime;
        public long herbsProdFirstStartFarmTime;
        public byte herbsProdFirstWorkerFirst;
        public byte herbsProdFirstWorkerSecond;

        public byte herbsProdSecondLevel;
        public long herbsProdSecondStartConstructionTime;
        public long herbsProdSecondStartFarmTime;
        public byte herbsProdSecondWorkerFirst;
        public byte herbsProdSecondWorkerSecond;

        public byte herbsProdThirdLevel;
        public long herbsProdThirdStartConstructionTime;
        public long herbsProdThirdStartFarmTime;
        public byte herbsProdThirdWorkerFirst;
        public byte herbsProdThirdWorkerSecond;

        // --- Добыча ресурсов: Дерево
        public byte woodProdFirstLevel;
        public long woodProdFirstStartConstructionTime;
        public long woodProdFirstStartFarmTime;
        public byte woodProdFirstWorkerFirst;
        public byte woodProdFirstWorkerSecond;

        public byte woodProdSecondLevel;
        public long woodProdSecondStartConstructionTime;
        public long woodProdSecondStartFarmTime;
        public byte woodProdSecondWorkerFirst;
        public byte woodProdSecondWorkerSecond;

        // --- Тренировка
        public byte warriorTrainingLevel;
        public long warriorTrainingStartConstructionTime;
        public sbyte warriorTrainingFirstUnitIndex;
        public long warriorTrainingFirstUnitStartTime;

        public byte goldTrainingLevel;
        public long goldTrainingStartConstructionTime;
        public sbyte goldTrainingFirstUnitIndex;
        public sbyte goldTrainingSecondUnitIndex;
        public long goldTrainingFirstUnitStartTime;
        public long goldTrainingSecondUnitStartTime;

        public byte foodTrainingLevel;
        public long foodTrainingStartConstructionTime;
        public sbyte foodTrainingFirstUnitIndex;
        public sbyte foodTrainingSecondUnitIndex;
        public long foodTrainingFirstUnitStartTime;
        public long foodTrainingSecondUnitStartTime;

        public byte herbsTrainingLevel;
        public long herbsTrainingStartConstructionTime;
        public sbyte herbsTrainingFirstUnitIndex;
        public sbyte herbsTrainingSecondUnitIndex;
        public long herbsTrainingFirstUnitStartTime;
        public long herbsTrainingSecondUnitStartTime;

        public byte woodTrainingLevel;
        public long woodTrainingStartConstructionTime;
        public sbyte woodTrainingFirstUnitIndex;
        public sbyte woodTrainingSecondUnitIndex;
        public long woodTrainingFirstUnitStartTime;
        public long woodTrainingSecondUnitStartTime;



        public static TableColumn[] GetTableColumns()
        {
            return new TableColumn[]
            {
                new TableColumn("dbid", "INTEGER PRIMARY KEY AUTOINCREMENT", "0"),

                new TableColumn("townHallLevel", "INTEGER", "1"),
                new TableColumn("townHallStartConstructionTime", "INTEGER", "0"),
                new TableColumn("tyrLevel", "INTEGER", "1"),
                new TableColumn("tyrConstructionTime", "INTEGER", "0"),
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
                new TableColumn("goldProdFirstWorkerFirst", "INTEGER", "1"),
                new TableColumn("goldProdFirstWorkerSecond", "INTEGER", "1"),

                new TableColumn("goldProdSecondLevel", "INTEGER", "0"),
                new TableColumn("goldProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("goldProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("goldProdSecondWorkerFirst", "INTEGER", "1"),
                new TableColumn("goldProdSecondWorkerSecond", "INTEGER", "1"),

                new TableColumn("goldProdThirdLevel", "INTEGER", "0"),
                new TableColumn("goldProdThirdStartConstructionTime", "INTEGER", "0"),
                new TableColumn("goldProdThirdStartFarmTime", "INTEGER", "0"),
                new TableColumn("goldProdThirdWorkerFirst", "INTEGER", "1"),
                new TableColumn("goldProdThirdWorkerSecond", "INTEGER", "1"),

                // --- Добыча ресурсов: Еда
                new TableColumn("foodProdFirstLevel", "INTEGER", "0"),
                new TableColumn("foodProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("foodProdFirstWorkerFirst", "INTEGER", "1"),
                new TableColumn("foodProdFirstWorkerSecond", "INTEGER", "1"),

                new TableColumn("foodProdSecondLevel", "INTEGER", "0"),
                new TableColumn("foodProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("foodProdSecondWorkerFirst", "INTEGER", "1"),
                new TableColumn("foodProdSecondWorkerSecond", "INTEGER", "1"),

                new TableColumn("foodProdThirdLevel", "INTEGER", "0"),
                new TableColumn("foodProdThirdStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodProdThirdStartFarmTime", "INTEGER", "0"),
                new TableColumn("foodProdThirdWorkerFirst", "INTEGER", "1"),
                new TableColumn("foodProdThirdWorkerSecond", "INTEGER", "1"),

                // --- Добыча ресурсов: Травы
                new TableColumn("herbsProdFirstLevel", "INTEGER", "0"),
                new TableColumn("herbsProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("herbsProdFirstWorkerFirst", "INTEGER", "1"),
                new TableColumn("herbsProdFirstWorkerSecond", "INTEGER", "1"),

                new TableColumn("herbsProdSecondLevel", "INTEGER", "0"),
                new TableColumn("herbsProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("herbsProdSecondWorkerFirst", "INTEGER", "1"),
                new TableColumn("herbsProdSecondWorkerSecond", "INTEGER", "1"),

                new TableColumn("herbsProdThirdLevel", "INTEGER", "0"),
                new TableColumn("herbsProdThirdStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsProdThirdStartFarmTime", "INTEGER", "0"),
                new TableColumn("herbsProdThirdWorkerFirst", "INTEGER", "1"),
                new TableColumn("herbsProdThirdWorkerSecond", "INTEGER", "1"),

                // --- Добыча ресурсов: Дерево
                new TableColumn("woodProdFirstLevel", "INTEGER", "0"),
                new TableColumn("woodProdFirstStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodProdFirstStartFarmTime", "INTEGER", "0"),
                new TableColumn("woodProdFirstWorkerFirst", "INTEGER", "1"),
                new TableColumn("woodProdFirstWorkerSecond", "INTEGER", "1"),

                new TableColumn("woodProdSecondLevel", "INTEGER", "0"),
                new TableColumn("woodProdSecondStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodProdSecondStartFarmTime", "INTEGER", "0"),
                new TableColumn("woodProdSecondWorkerFirst", "INTEGER", "1"),
                new TableColumn("woodProdSecondWorkerSecond", "INTEGER", "1"),

                // --- Тренировка
                new TableColumn("warriorTrainingLevel", "INTEGER", "0"),
                new TableColumn("warriorTrainingStartConstructionTime", "INTEGER", "0"),
                new TableColumn("warriorTrainingFirstUnitIndex", "INTEGER", "-1"),
                new TableColumn("warriorTrainingFirstUnitStartTime", "INTEGER", "0"),

                new TableColumn("goldTrainingLevel", "INTEGER", "0"),
                new TableColumn("goldTrainingStartConstructionTime", "INTEGER", "0"),
                new TableColumn("goldTrainingFirstUnitIndex", "INTEGER", "-1"),
                new TableColumn("goldTrainingSecondUnitIndex", "INTEGER", "-1"),
                new TableColumn("goldTrainingFirstUnitStartTime", "INTEGER", "0"),
                new TableColumn("goldTrainingSecondUnitStartTime", "INTEGER", "0"),

                new TableColumn("foodTrainingLevel", "INTEGER", "0"),
                new TableColumn("foodTrainingStartConstructionTime", "INTEGER", "0"),
                new TableColumn("foodTrainingFirstUnitIndex", "INTEGER", "-1"),
                new TableColumn("foodTrainingSecondUnitIndex", "INTEGER", "-1"),
                new TableColumn("foodTrainingFirstUnitStartTime", "INTEGER", "0"),
                new TableColumn("foodTrainingSecondUnitStartTime", "INTEGER", "0"),

                new TableColumn("herbsTrainingLevel", "INTEGER", "0"),
                new TableColumn("herbsTrainingStartConstructionTime", "INTEGER", "0"),
                new TableColumn("herbsTrainingFirstUnitIndex", "INTEGER", "-1"),
                new TableColumn("herbsTrainingSecondUnitIndex", "INTEGER", "-1"),
                new TableColumn("herbsTrainingFirstUnitStartTime", "INTEGER", "0"),
                new TableColumn("herbsTrainingSecondUnitStartTime", "INTEGER", "0"),

                new TableColumn("woodTrainingLevel", "INTEGER", "0"),
                new TableColumn("woodTrainingStartConstructionTime", "INTEGER", "0"),
                new TableColumn("woodTrainingFirstUnitIndex", "INTEGER", "-1"),
                new TableColumn("woodTrainingSecondUnitIndex", "INTEGER", "-1"),
                new TableColumn("woodTrainingFirstUnitStartTime", "INTEGER", "0"),
                new TableColumn("woodTrainingSecondUnitStartTime", "INTEGER", "0"),
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
            var success = await buildingsTable.UpdateDataInDatabase(this).ConfigureAwait(false);
            return success;
        }




    }
}
