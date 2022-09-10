using TextGameRPG.Scripts.GameCore.Buildings.Data;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
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
        WoodProductionThird = 311,

        // --- Тренировочные залы
        FightTraining = 400,
        GoldTraining = 401,
        FoodTraining = 402,
        HerbsTraining = 403,
        WoodTraining = 404,
    }

    public static class BuildingTypeExtensions
    {
        public static BuildingLevelInfo CreateNewLevelInfo(this BuildingType buildingType)
        {
            switch (buildingType)
            {
                case BuildingType.GoldStorage:
                case BuildingType.FoodStorage:
                case BuildingType.HerbsStorage:
                case BuildingType.WoodStorage:
                    return new StorageLevelInfo();

                default:
                    return new BuildingLevelInfo();
            }
        }
    }
}
