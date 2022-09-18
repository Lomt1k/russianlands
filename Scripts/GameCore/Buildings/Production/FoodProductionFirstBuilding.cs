using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class FoodProductionFirstBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Food;
        public override BuildingType buildingType => BuildingType.FoodProductionFirst;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.foodProdFirstLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdFirstLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.foodProdFirstStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.foodProdFirstStartConstructionTime = startConstructionTime;
        }

        public override long GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.foodProdFirstStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime)
        {
            data.foodProdFirstStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.foodProdFirstWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.foodProdFirstWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdFirstWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdFirstWorkerSecond = level;
        }
    }
}
