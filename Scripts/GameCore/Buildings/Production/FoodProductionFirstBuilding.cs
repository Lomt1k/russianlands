using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Units;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class FoodProductionFirstBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Food;
        public override BuildingType buildingType => BuildingType.FoodProductionFirst;
        public override Avatar firstWorkerIcon => Avatar.FemaleB;
        public override Avatar secondWorkerIcon => Avatar.FemaleC;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.foodProdFirstLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdFirstLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.foodProdFirstStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.foodProdFirstStartConstructionTime = startConstructionTime;
        }

        public override DateTime GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.foodProdFirstStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
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
