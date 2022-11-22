using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

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

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.foodStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.foodStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.foodStorageStartConstructionTime = startConstructionTime;
        }
    }
}
