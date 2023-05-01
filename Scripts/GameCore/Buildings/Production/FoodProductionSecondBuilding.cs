using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Units;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class FoodProductionSecondBuilding : ProductionBuildingBase
    {
        public override ResourceId resourceId => ResourceId.Food;
        public override BuildingId buildingId => BuildingId.FoodProductionSecond;
        public override Avatar firstWorkerIcon => Avatar.MaleF;
        public override Avatar secondWorkerIcon => Avatar.MaleH;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.foodProdSecondLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdSecondLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.foodProdSecondStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.foodProdSecondStartConstructionTime = startConstructionTime;
        }

        public override DateTime GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.foodProdSecondStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
        {
            data.foodProdSecondStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.foodProdSecondWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.foodProdSecondWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdSecondWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.foodProdSecondWorkerSecond = level;
        }

    }
}
