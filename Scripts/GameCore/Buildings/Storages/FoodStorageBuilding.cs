using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages
{
    public class FoodStorageBuilding : StorageBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Food;
        public override BuildingType buildingType => BuildingType.FoodStorage;
        public override int resourceLimitForZeroLevel => 2_000;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.foodStorageLevel;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.foodStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.foodStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.foodStorageStartConstructionTime = startConstructionTime;
        }
    }
}
