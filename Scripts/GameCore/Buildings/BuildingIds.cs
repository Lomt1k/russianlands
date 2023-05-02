using System.Collections.Generic;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Buildings.Craft;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Buildings.Production;
using MarkOne.Scripts.GameCore.Buildings.Storages;
using MarkOne.Scripts.GameCore.Buildings.Training;

namespace MarkOne.Scripts.GameCore.Buildings;

public enum BuildingCategory
{
    General,
    Storages,
    Production,
    Training,
}

public enum BuildingId
{
    // --- Основные
    TownHall = 100, // Ратуша
    Tyr = 101, // Стрельбище
    Hospital = 102, // Здравница
    AlchemyLab = 103, // Мастерская зелий
    ElixirWorkshop = 104, // Мастерская эликсиров
    WeaponsWorkshop = 105, // Мастерская оружия
    ArmorWorkshop = 106, // Мастерская брони
    Jewerly = 107, // Ювелирная мастерская
    ScribesHouse = 108, // Дом писарей

    // --- Хранилища
    GoldStorage = 200,
    FoodStorage = 201,
    HerbsStorage = 202,
    WoodStorage = 203,

    ItemsStorage = 210,

    // --- Добыча ресурсов
    GoldProductionFirst = 300,
    GoldProductionSecond = 301,
    GoldProductionThird = 302,
    FoodProductionFirst = 303,
    FoodProductionSecond = 304,
    FoodProductionThird = 305,
    HerbsProductionFirst = 306,
    HerbsProductionSecond = 307,
    HerbsProductionThird = 308,
    WoodProductionFirst = 309,
    WoodProductionSecond = 310,

    // --- Тренировочные залы
    WarriorTraining = 400,
    GoldTraining = 401,
    FoodTraining = 402,
    HerbsTraining = 403,
    WoodTraining = 404,
}

public static class BuildingIdExtensions
{
    private static readonly Dictionary<BuildingId, BuildingBase> _dictionary = new Dictionary<BuildingId, BuildingBase>
    {
        // --- Основные
        { BuildingId.TownHall, new TownHallBuilding() },
        { BuildingId.Tyr, new TyrBuilding() },
        { BuildingId.Hospital, new HospitalBuilding() },
        { BuildingId.AlchemyLab, new AlchemyLabBuilding() },
        { BuildingId.ElixirWorkshop, new ElixirWorkshopBuilding() },
        { BuildingId.WeaponsWorkshop, new WeaponsWorkshopBuilding() },
        { BuildingId.ArmorWorkshop, new ArmorWorkshopBuilding() },
        { BuildingId.Jewerly, new JewerlyBuilding() },
        { BuildingId.ScribesHouse, new ScribesHouseBuilding() },

        // --- Хранилища
        { BuildingId.ItemsStorage, new ItemsStorageBuilding() },
        { BuildingId.GoldStorage, new GoldStorageBuilding() },
        { BuildingId.FoodStorage, new FoodStorageBuilding() },
        { BuildingId.HerbsStorage, new HerbsStorageBuilding() },
        { BuildingId.WoodStorage, new WoodStorageBuilding() },

        // --- Добыча ресурсов
        { BuildingId.GoldProductionFirst, new GoldProductionFirstBuilding() },
        { BuildingId.GoldProductionSecond, new GoldProductionSecondBuilding() },
        { BuildingId.GoldProductionThird, new GoldProductionThirdBuilding() },
        { BuildingId.FoodProductionFirst, new FoodProductionFirstBuilding() },
        { BuildingId.FoodProductionSecond, new FoodProductionSecondBuilding() },
        { BuildingId.FoodProductionThird, new FoodProductionThirdBuilding() },
        { BuildingId.HerbsProductionFirst, new HerbsProductionFirstBuilding() },
        { BuildingId.HerbsProductionSecond, new HerbsProductionSecondBuilding() },
        { BuildingId.HerbsProductionThird, new HerbsProductionThirdBuilding() },
        { BuildingId.WoodProductionFirst, new WoodProductionFirstBuilding() },
        { BuildingId.WoodProductionSecond, new WoodProductionSecondBuilding() },

        // --- Тренировочные залы
        { BuildingId.WarriorTraining, new WarriorTrainingBuilding() },
        { BuildingId.GoldTraining, new GoldTrainingBuilding() },
        { BuildingId.FoodTraining, new FoodTrainingBuilding() },
        { BuildingId.HerbsTraining, new HerbsTrainingBuilding() },
        { BuildingId.WoodTraining, new WoodTrainingBuilding() },
    };

    public static BuildingLevelInfo CreateNewLevelInfo(this BuildingId buildingId)
    {
        switch (buildingId)
        {
            case BuildingId.ItemsStorage:
            case BuildingId.GoldStorage:
            case BuildingId.FoodStorage:
            case BuildingId.HerbsStorage:
            case BuildingId.WoodStorage:
                return new StorageLevelInfo();

            case BuildingId.GoldProductionFirst:
            case BuildingId.GoldProductionSecond:
            case BuildingId.GoldProductionThird:
            case BuildingId.FoodProductionFirst:
            case BuildingId.FoodProductionSecond:
            case BuildingId.FoodProductionThird:
            case BuildingId.HerbsProductionFirst:
            case BuildingId.HerbsProductionSecond:
            case BuildingId.HerbsProductionThird:
            case BuildingId.WoodProductionFirst:
            case BuildingId.WoodProductionSecond:
                return new ProductionLevelInfo();

            case BuildingId.WarriorTraining:
            case BuildingId.GoldTraining:
            case BuildingId.FoodTraining:
            case BuildingId.HerbsTraining:
            case BuildingId.WoodTraining:
                return new TrainingLevelInfo();

            case BuildingId.WeaponsWorkshop:
            case BuildingId.ArmorWorkshop:
            case BuildingId.Jewerly:
            case BuildingId.ScribesHouse:
                return new CraftLevelInfo();

            case BuildingId.Hospital:
                return new HospitalLevelInfo();

            case BuildingId.Tyr:
                return new TyrLevelInfo();

            case BuildingId.AlchemyLab:
                return new AlchemyLabLevelInfo();

            case BuildingId.ElixirWorkshop:
                return new ElixirWorkshopLevelInfo();

            default:
                return new BuildingLevelInfo();
        }
    }

    public static BuildingBase GetBuilding(this BuildingId buildingId)
    {
        return _dictionary[buildingId];
    }

    public static string GetLocalization(this BuildingCategory category, GameSession session)
    {
        return Localizations.Localization.Get(session, "building_category_" + category.ToString().ToLower());
    }

    public static BuildingCategory GetCategory(this BuildingId buildingId)
    {
        switch (buildingId)
        {
            case BuildingId.ItemsStorage:
            case BuildingId.GoldStorage:
            case BuildingId.FoodStorage:
            case BuildingId.HerbsStorage:
            case BuildingId.WoodStorage:
                return BuildingCategory.Storages;

            case BuildingId.GoldProductionFirst:
            case BuildingId.GoldProductionSecond:
            case BuildingId.GoldProductionThird:
            case BuildingId.FoodProductionFirst:
            case BuildingId.FoodProductionSecond:
            case BuildingId.FoodProductionThird:
            case BuildingId.HerbsProductionFirst:
            case BuildingId.HerbsProductionSecond:
            case BuildingId.HerbsProductionThird:
            case BuildingId.WoodProductionFirst:
            case BuildingId.WoodProductionSecond:
                return BuildingCategory.Production;

            case BuildingId.WarriorTraining:
            case BuildingId.GoldTraining:
            case BuildingId.FoodTraining:
            case BuildingId.HerbsTraining:
            case BuildingId.WoodTraining:
                return BuildingCategory.Training;

            default:
                return BuildingCategory.General;
        }
    }

}
