using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages
{
    public class HerbsStorageBuilding : StorageBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Herbs;
        public override BuildingType buildingType => BuildingType.HerbsStorage;
        public override int resourceLimitForZeroLevel => 10_000;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.herbsStorageLevel;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.herbsStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.herbsStorageStartConstructionTime = startConstructionTime;
        }
    }
}
