using SQLite;

namespace TextGameRPG.Scripts.Bot.DataBase.SerializableData
{
    [Table("Buildings")]
    public class ProfileBuildingsData : DataWithSession
    {
        [PrimaryKey]
        public long dbid { get; set; }

        public byte townHallLevel { get; set; } = 1;
        public long townHallStartConstructionTime { get; set; }
        public byte tyrLevel { get; set; } = 1;
        public long tyrStartConstructionTime { get; set; }
        public byte hospitalLevel { get; set; } = 1;
        public long hospitalStartConstructionTime { get; set; }
        public long hospitalLastHealthRestoreTime { get; set; }
        public byte potionWorkshopLevel { get; set; }
        public long potionWorkshopStartConstructionTime { get; set; }
        public byte elixirWorkshopLevel { get; set; }
        public long elixirWorkshopStartConstructionTime { get; set; }

        public byte weaponsWorkshopLevel { get; set; }
        public long weaponsWorkshopStartConstructionTime { get; set; }
        public long weaponsWorkshopStartCraftTime { get; set; }
        public sbyte weaponsWorkshopCraftItemType { get; set; }
        public byte weaponsWorkshopCraftItemRarity { get; set; }
        public byte armorWorkshopLevel { get; set; }
        public long armorWorkshopStartConstructionTime { get; set; }
        public long armorWorkshopStartCraftTime { get; set; }
        public sbyte armorWorkshopCraftItemType { get; set; }
        public byte armorWorkshopCraftItemRarity { get; set; }
        public byte jewerlyLevel { get; set; }
        public long jewerlyStartConstructionTime { get; set; }
        public long jewerlyStartCraftTime { get; set; }
        public sbyte jewerlyCraftItemType { get; set; }
        public byte jewerlyCraftItemRarity { get; set; }
        public byte scribesHouseLevel { get; set; }
        public long scribesHouseStartConstructionTime { get; set; }
        public long scribesHouseStartCraftTime { get; set; }
        public byte scribesHouseCraftItemRarity { get; set; }

        // --- Хранилища
        public byte goldStorageLevel { get; set; } = 1;
        public long goldStorageStartConstructionTime { get; set; }
        public byte foodStorageLevel { get; set; }
        public long foodStorageStartConstructionTime { get; set; }
        public byte herbsStorageLevel { get; set; }
        public long herbsStorageStartConstructionTime { get; set; }
        public byte woodStorageLevel { get; set; }
        public long woodStorageStartConstructionTime { get; set; }
        public byte itemsStorageLevel { get; set; } = 1;
        public long itemsStorageStartConstructionTime { get; set; }

        // --- Добыча ресурсов: Золото
        public byte goldProdFirstLevel { get; set; } = 1;
        public long goldProdFirstStartConstructionTime { get; set; }
        public long goldProdFirstStartFarmTime { get; set; }
        public byte goldProdFirstWorkerFirst { get; set; } = 1;
        public byte goldProdFirstWorkerSecond { get; set; } = 1;

        public byte goldProdSecondLevel { get; set; }
        public long goldProdSecondStartConstructionTime { get; set; }
        public long goldProdSecondStartFarmTime { get; set; }
        public byte goldProdSecondWorkerFirst { get; set; } = 1;
        public byte goldProdSecondWorkerSecond { get; set; } = 1;

        public byte goldProdThirdLevel { get; set; }
        public long goldProdThirdStartConstructionTime { get; set; }
        public long goldProdThirdStartFarmTime { get; set; }
        public byte goldProdThirdWorkerFirst { get; set; } = 1;
        public byte goldProdThirdWorkerSecond { get; set; } = 1;

        // --- Добыча ресурсов: Еда
        public byte foodProdFirstLevel { get; set; }
        public long foodProdFirstStartConstructionTime { get; set; }
        public long foodProdFirstStartFarmTime { get; set; }
        public byte foodProdFirstWorkerFirst { get; set; } = 1;
        public byte foodProdFirstWorkerSecond { get; set; } = 1;

        public byte foodProdSecondLevel { get; set; }
        public long foodProdSecondStartConstructionTime { get; set; }
        public long foodProdSecondStartFarmTime { get; set; }
        public byte foodProdSecondWorkerFirst { get; set; } = 1;
        public byte foodProdSecondWorkerSecond { get; set; } = 1;

        public byte foodProdThirdLevel { get; set; }
        public long foodProdThirdStartConstructionTime { get; set; }
        public long foodProdThirdStartFarmTime { get; set; }
        public byte foodProdThirdWorkerFirst { get; set; } = 1;
        public byte foodProdThirdWorkerSecond { get; set; } = 1;

        // --- Добыча ресурсов: Травы
        public byte herbsProdFirstLevel { get; set; }
        public long herbsProdFirstStartConstructionTime { get; set; }
        public long herbsProdFirstStartFarmTime { get; set; }
        public byte herbsProdFirstWorkerFirst { get; set; } = 1;
        public byte herbsProdFirstWorkerSecond { get; set; } = 1;

        public byte herbsProdSecondLevel { get; set; }
        public long herbsProdSecondStartConstructionTime { get; set; }
        public long herbsProdSecondStartFarmTime { get; set; }
        public byte herbsProdSecondWorkerFirst { get; set; } = 1;
        public byte herbsProdSecondWorkerSecond { get; set; } = 1;

        public byte herbsProdThirdLevel { get; set; }
        public long herbsProdThirdStartConstructionTime { get; set; }
        public long herbsProdThirdStartFarmTime { get; set; }
        public byte herbsProdThirdWorkerFirst { get; set; } = 1;
        public byte herbsProdThirdWorkerSecond { get; set; } = 1;

        // --- Добыча ресурсов: Дерево
        public byte woodProdFirstLevel { get; set; }
        public long woodProdFirstStartConstructionTime { get; set; }
        public long woodProdFirstStartFarmTime { get; set; }
        public byte woodProdFirstWorkerFirst { get; set; } = 1;
        public byte woodProdFirstWorkerSecond { get; set; } = 1;

        public byte woodProdSecondLevel { get; set; }
        public long woodProdSecondStartConstructionTime { get; set; }
        public long woodProdSecondStartFarmTime { get; set; }
        public byte woodProdSecondWorkerFirst { get; set; } = 1;
        public byte woodProdSecondWorkerSecond { get; set; } = 1;

        // --- Тренировка
        public byte warriorTrainingLevel { get; set; }
        public long warriorTrainingStartConstructionTime { get; set; }
        public sbyte warriorTrainingFirstUnitIndex { get; set; } = -1;
        public long warriorTrainingFirstUnitStartTime { get; set; }

        public byte goldTrainingLevel { get; set; }
        public long goldTrainingStartConstructionTime { get; set; }
        public sbyte goldTrainingFirstUnitIndex { get; set; } = -1;
        public sbyte goldTrainingSecondUnitIndex { get; set; } = -1;
        public long goldTrainingFirstUnitStartTime { get; set; }
        public long goldTrainingSecondUnitStartTime { get; set; }

        public byte foodTrainingLevel { get; set; }
        public long foodTrainingStartConstructionTime { get; set; }
        public sbyte foodTrainingFirstUnitIndex { get; set; } = -1;
        public sbyte foodTrainingSecondUnitIndex { get; set; } = -1;
        public long foodTrainingFirstUnitStartTime { get; set; }
        public long foodTrainingSecondUnitStartTime { get; set; }

        public byte herbsTrainingLevel { get; set; }
        public long herbsTrainingStartConstructionTime { get; set; }
        public sbyte herbsTrainingFirstUnitIndex { get; set; } = -1;
        public sbyte herbsTrainingSecondUnitIndex { get; set; } = -1;
        public long herbsTrainingFirstUnitStartTime { get; set; }
        public long herbsTrainingSecondUnitStartTime { get; set; }

        public byte woodTrainingLevel { get; set; }
        public long woodTrainingStartConstructionTime { get; set; }
        public sbyte woodTrainingFirstUnitIndex { get; set; } = -1;
        public sbyte woodTrainingSecondUnitIndex { get; set; } = -1;
        public long woodTrainingFirstUnitStartTime { get; set; }
        public long woodTrainingSecondUnitStartTime { get; set; }


        //public async override Task<bool> UpdateInDatabase()
        //{
        //    if (!isDeserializationCompleted)
        //        return true;

        //    var buildingsTable = BotController.dataBase[Table.ProfileBuildings] as ProfileBuildingsDataTable;
        //    var success = await buildingsTable.UpdateDataInDatabase(this).FastAwait();
        //    return success;
        //}




    }
}
