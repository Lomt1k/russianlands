using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Buildings.Production;
using TextGameRPG.Scripts.GameCore.Buildings.Storages;
using TextGameRPG.Scripts.GameCore.Buildings.Training;
using TextGameRPG.Scripts.TelegramBot.Sessions;

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
        Barracks = 101, // Казарма
        Hospital = 102, // Здравница
        AlchemyShop = 103, // Алхимическая лавка
        Laboratory = 104, // Лаборатория
        WeaponForge = 105, // Кузница оружия
        ArmorForge = 106, // Кузница доспехов
        Jewerly = 107, // Ювелирная
        Workshop = 108, // Мастерская

        // --- Хранилища
        GoldStorage = 200,
        FoodStorage = 201,
        HerbsStorage = 202,
        WoodStorage = 203,

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

            // --- Хранилища
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
            { BuildingType.GoldTraining, new GoldTrainingBuilding() },
            { BuildingType.FoodTraining, new FoodTrainingBuilding() },
            { BuildingType.HerbsTraining, new HerbsTrainingBuilding() },
            { BuildingType.WoodTraining, new WoodTrainingBuilding() },
        };

        public static BuildingLevelInfo CreateNewLevelInfo(this BuildingType buildingType)
        {
            switch (buildingType)
            {
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

                case BuildingType.WeaponForge:
                case BuildingType.ArmorForge:
                case BuildingType.Jewerly:
                    return new CraftLevelInfo();

                case BuildingType.Hospital:
                    return new HospitalLevelInfo();

                case BuildingType.Barracks:
                    return new BarracksLevelInfo();

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
