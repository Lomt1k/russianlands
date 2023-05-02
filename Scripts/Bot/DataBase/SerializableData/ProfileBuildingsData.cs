using SQLite;
using System;
using MarkOne.Scripts.GameCore.Items;

namespace MarkOne.Scripts.Bot.DataBase.SerializableData;

[Table("Buildings")]
public class ProfileBuildingsData : DataWithSession
{
    [PrimaryKey]
    public long dbid { get; set; }

    public byte townHallLevel { get; set; } = 1;
    public DateTime townHallStartConstructionTime { get; set; }
    public byte tyrLevel { get; set; } = 1;
    public DateTime tyrStartConstructionTime { get; set; }
    public byte hospitalLevel { get; set; } = 1;
    public DateTime hospitalStartConstructionTime { get; set; }
    public DateTime hospitalLastHealthRestoreTime { get; set; }
    public byte potionWorkshopLevel { get; set; }
    public DateTime potionWorkshopStartConstructionTime { get; set; }
    public byte elixirWorkshopLevel { get; set; }
    public DateTime elixirWorkshopStartConstructionTime { get; set; }

    public byte weaponsWorkshopLevel { get; set; }
    public DateTime weaponsWorkshopStartConstructionTime { get; set; }
    public DateTime weaponsWorkshopStartCraftTime { get; set; }
    public ItemType weaponsWorkshopCraftItemType { get; set; }
    public Rarity weaponsWorkshopCraftItemRarity { get; set; }
    public byte armorWorkshopLevel { get; set; }
    public DateTime armorWorkshopStartConstructionTime { get; set; }
    public DateTime armorWorkshopStartCraftTime { get; set; }
    public ItemType armorWorkshopCraftItemType { get; set; }
    public Rarity armorWorkshopCraftItemRarity { get; set; }
    public byte jewerlyLevel { get; set; }
    public DateTime jewerlyStartConstructionTime { get; set; }
    public DateTime jewerlyStartCraftTime { get; set; }
    public ItemType jewerlyCraftItemType { get; set; }
    public Rarity jewerlyCraftItemRarity { get; set; }
    public byte scribesHouseLevel { get; set; }
    public DateTime scribesHouseStartConstructionTime { get; set; }
    public DateTime scribesHouseStartCraftTime { get; set; }
    public Rarity scribesHouseCraftItemRarity { get; set; }

    // --- Хранилища
    public byte goldStorageLevel { get; set; } = 1;
    public DateTime goldStorageStartConstructionTime { get; set; }
    public byte foodStorageLevel { get; set; }
    public DateTime foodStorageStartConstructionTime { get; set; }
    public byte herbsStorageLevel { get; set; }
    public DateTime herbsStorageStartConstructionTime { get; set; }
    public byte woodStorageLevel { get; set; }
    public DateTime woodStorageStartConstructionTime { get; set; }
    public byte itemsStorageLevel { get; set; } = 1;
    public DateTime itemsStorageStartConstructionTime { get; set; }

    // --- Добыча ресурсов: Золото
    public byte goldProdFirstLevel { get; set; } = 1;
    public DateTime goldProdFirstStartConstructionTime { get; set; }
    public DateTime goldProdFirstStartFarmTime { get; set; }
    public byte goldProdFirstWorkerFirst { get; set; } = 1;
    public byte goldProdFirstWorkerSecond { get; set; } = 1;

    public byte goldProdSecondLevel { get; set; }
    public DateTime goldProdSecondStartConstructionTime { get; set; }
    public DateTime goldProdSecondStartFarmTime { get; set; }
    public byte goldProdSecondWorkerFirst { get; set; } = 1;
    public byte goldProdSecondWorkerSecond { get; set; } = 1;

    public byte goldProdThirdLevel { get; set; }
    public DateTime goldProdThirdStartConstructionTime { get; set; }
    public DateTime goldProdThirdStartFarmTime { get; set; }
    public byte goldProdThirdWorkerFirst { get; set; } = 1;
    public byte goldProdThirdWorkerSecond { get; set; } = 1;

    // --- Добыча ресурсов: Еда
    public byte foodProdFirstLevel { get; set; }
    public DateTime foodProdFirstStartConstructionTime { get; set; }
    public DateTime foodProdFirstStartFarmTime { get; set; }
    public byte foodProdFirstWorkerFirst { get; set; } = 1;
    public byte foodProdFirstWorkerSecond { get; set; } = 1;

    public byte foodProdSecondLevel { get; set; }
    public DateTime foodProdSecondStartConstructionTime { get; set; }
    public DateTime foodProdSecondStartFarmTime { get; set; }
    public byte foodProdSecondWorkerFirst { get; set; } = 1;
    public byte foodProdSecondWorkerSecond { get; set; } = 1;

    public byte foodProdThirdLevel { get; set; }
    public DateTime foodProdThirdStartConstructionTime { get; set; }
    public DateTime foodProdThirdStartFarmTime { get; set; }
    public byte foodProdThirdWorkerFirst { get; set; } = 1;
    public byte foodProdThirdWorkerSecond { get; set; } = 1;

    // --- Добыча ресурсов: Травы
    public byte herbsProdFirstLevel { get; set; }
    public DateTime herbsProdFirstStartConstructionTime { get; set; }
    public DateTime herbsProdFirstStartFarmTime { get; set; }
    public byte herbsProdFirstWorkerFirst { get; set; } = 1;
    public byte herbsProdFirstWorkerSecond { get; set; } = 1;

    public byte herbsProdSecondLevel { get; set; }
    public DateTime herbsProdSecondStartConstructionTime { get; set; }
    public DateTime herbsProdSecondStartFarmTime { get; set; }
    public byte herbsProdSecondWorkerFirst { get; set; } = 1;
    public byte herbsProdSecondWorkerSecond { get; set; } = 1;

    public byte herbsProdThirdLevel { get; set; }
    public DateTime herbsProdThirdStartConstructionTime { get; set; }
    public DateTime herbsProdThirdStartFarmTime { get; set; }
    public byte herbsProdThirdWorkerFirst { get; set; } = 1;
    public byte herbsProdThirdWorkerSecond { get; set; } = 1;

    // --- Добыча ресурсов: Дерево
    public byte woodProdFirstLevel { get; set; }
    public DateTime woodProdFirstStartConstructionTime { get; set; }
    public DateTime woodProdFirstStartFarmTime { get; set; }
    public byte woodProdFirstWorkerFirst { get; set; } = 1;
    public byte woodProdFirstWorkerSecond { get; set; } = 1;

    public byte woodProdSecondLevel { get; set; }
    public DateTime woodProdSecondStartConstructionTime { get; set; }
    public DateTime woodProdSecondStartFarmTime { get; set; }
    public byte woodProdSecondWorkerFirst { get; set; } = 1;
    public byte woodProdSecondWorkerSecond { get; set; } = 1;

    // --- Тренировка
    public byte warriorTrainingLevel { get; set; }
    public DateTime warriorTrainingStartConstructionTime { get; set; }
    public sbyte warriorTrainingFirstUnitIndex { get; set; } = -1;
    public DateTime warriorTrainingFirstUnitStartTime { get; set; }

    public byte goldTrainingLevel { get; set; }
    public DateTime goldTrainingStartConstructionTime { get; set; }
    public sbyte goldTrainingFirstUnitIndex { get; set; } = -1;
    public sbyte goldTrainingSecondUnitIndex { get; set; } = -1;
    public DateTime goldTrainingFirstUnitStartTime { get; set; }
    public DateTime goldTrainingSecondUnitStartTime { get; set; }

    public byte foodTrainingLevel { get; set; }
    public DateTime foodTrainingStartConstructionTime { get; set; }
    public sbyte foodTrainingFirstUnitIndex { get; set; } = -1;
    public sbyte foodTrainingSecondUnitIndex { get; set; } = -1;
    public DateTime foodTrainingFirstUnitStartTime { get; set; }
    public DateTime foodTrainingSecondUnitStartTime { get; set; }

    public byte herbsTrainingLevel { get; set; }
    public DateTime herbsTrainingStartConstructionTime { get; set; }
    public sbyte herbsTrainingFirstUnitIndex { get; set; } = -1;
    public sbyte herbsTrainingSecondUnitIndex { get; set; } = -1;
    public DateTime herbsTrainingFirstUnitStartTime { get; set; }
    public DateTime herbsTrainingSecondUnitStartTime { get; set; }

    public byte woodTrainingLevel { get; set; }
    public DateTime woodTrainingStartConstructionTime { get; set; }
    public sbyte woodTrainingFirstUnitIndex { get; set; } = -1;
    public sbyte woodTrainingSecondUnitIndex { get; set; } = -1;
    public DateTime woodTrainingFirstUnitStartTime { get; set; }
    public DateTime woodTrainingSecondUnitStartTime { get; set; }

}
