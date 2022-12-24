using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Buildings.Production;
using TextGameRPG.Scripts.GameCore.Buildings.Storages;
using TextGameRPG.Scripts.GameCore.Buildings.Training;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public enum BuildingCategory 
    {
        General,
        Storages,
        Production,
        Training,
    }

    public enum BuildingType
    {
        // --- Основные
        TownHall = 100, // Ратуша
        Tyr = 101, // Стрельбище
        Hospital = 102, // Здравница
        PotionWorkshop = 103, // Мастерская зелий
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

    public static class BuildingTypeExtensions
    {
        private static Dictionary<BuildingType, BuildingBase> _dictionary = new Dictionary<BuildingType, BuildingBase>
        {
            // --- Основные
            { BuildingType.TownHall, new TownHallBuilding() },
            { BuildingType.Tyr, new TyrBuilding() },
            { BuildingType.Hospital, new HospitalBuilding() },

            // --- Хранилища
            { BuildingType.ItemsStorage, new ItemsStorageBuilding() },
            { BuildingType.GoldStorage, new GoldStorageBuilding() },
            { BuildingType.FoodStorage, new FoodStorageBuilding() },
            { BuildingType.HerbsStorage, new HerbsStorageBuilding() },
            { BuildingType.WoodStorage, new WoodStorageBuilding() },

            // --- Добыча ресурсов
            { BuildingType.GoldProductionFirst, new GoldProductionFirstBuilding() },
            { BuildingType.GoldProductionSecond, new GoldProductionSecondBuilding() },
            { BuildingType.GoldProductionThird, new GoldProductionThirdBuilding() },
            { BuildingType.FoodProductionFirst, new FoodProductionFirstBuilding() },
            { BuildingType.FoodProductionSecond, new FoodProductionSecondBuilding() },
            { BuildingType.FoodProductionThird, new FoodProductionThirdBuilding() },
            { BuildingType.HerbsProductionFirst, new HerbsProductionFirstBuilding() },
            { BuildingType.HerbsProductionSecond, new HerbsProductionSecondBuilding() },
            { BuildingType.HerbsProductionThird, new HerbsProductionThirdBuilding() },
            { BuildingType.WoodProductionFirst, new WoodProductionFirstBuilding() },
            { BuildingType.WoodProductionSecond, new WoodProductionSecondBuilding() },

            // --- Тренировочные залы
            { BuildingType.WarriorTraining, new WarriorTrainingBuilding() },
            { BuildingType.GoldTraining, new GoldTrainingBuilding() },
            { BuildingType.FoodTraining, new FoodTrainingBuilding() },
            { BuildingType.HerbsTraining, new HerbsTrainingBuilding() },
            { BuildingType.WoodTraining, new WoodTrainingBuilding() },
        };

        public static BuildingLevelInfo CreateNewLevelInfo(this BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.ItemsStorage:
                case BuildingType.GoldStorage:
                case BuildingType.FoodStorage:
                case BuildingType.HerbsStorage:
                case BuildingType.WoodStorage:
                    return new StorageLevelInfo();

                case BuildingType.GoldProductionFirst:
                case BuildingType.GoldProductionSecond:
                case BuildingType.GoldProductionThird:
                case BuildingType.FoodProductionFirst:
                case BuildingType.FoodProductionSecond:
                case BuildingType.FoodProductionThird:
                case BuildingType.HerbsProductionFirst:
                case BuildingType.HerbsProductionSecond:
                case BuildingType.HerbsProductionThird:
                case BuildingType.WoodProductionFirst:
                case BuildingType.WoodProductionSecond:
                    return new ProductionLevelInfo();

                case BuildingType.WarriorTraining:
                case BuildingType.GoldTraining:
                case BuildingType.FoodTraining:
                case BuildingType.HerbsTraining:
                case BuildingType.WoodTraining:
                    return new TrainingLevelInfo();

                case BuildingType.WeaponsWorkshop:
                case BuildingType.ArmorWorkshop:
                case BuildingType.Jewerly:
                    return new CraftLevelInfo();

                case BuildingType.Hospital:
                    return new HospitalLevelInfo();

                case BuildingType.Tyr:
                    return new TyrLevelInfo();

                default:
                    return new BuildingLevelInfo();
            }
        }

        public static BuildingBase GetBuilding(this BuildingType buildingType)
        {
            return _dictionary[buildingType];
        }

        public static string GetLocalization(this BuildingCategory category, GameSession session)
        {
            return Localizations.Localization.Get(session, "building_category_" + category.ToString().ToLower());
        }

        public static BuildingCategory GetCategory(this BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.ItemsStorage:
                case BuildingType.GoldStorage:
                case BuildingType.FoodStorage:
                case BuildingType.HerbsStorage:
                case BuildingType.WoodStorage:
                    return BuildingCategory.Storages;

                case BuildingType.GoldProductionFirst:
                case BuildingType.GoldProductionSecond:
                case BuildingType.GoldProductionThird:
                case BuildingType.FoodProductionFirst:
                case BuildingType.FoodProductionSecond:
                case BuildingType.FoodProductionThird:
                case BuildingType.HerbsProductionFirst:
                case BuildingType.HerbsProductionSecond:
                case BuildingType.HerbsProductionThird:
                case BuildingType.WoodProductionFirst:
                case BuildingType.WoodProductionSecond:
                    return BuildingCategory.Production;

                case BuildingType.WarriorTraining:
                case BuildingType.GoldTraining:
                case BuildingType.FoodTraining:
                case BuildingType.HerbsTraining:
                case BuildingType.WoodTraining:
                    return BuildingCategory.Training;

                default:
                    return BuildingCategory.General;
            }
        }

    }
}
