using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages
{
    public class WoodStorageBuilding : StorageBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Wood;
        public override BuildingType buildingType => BuildingType.WoodStorage;
        public override int resourceLimitForZeroLevel => 50_000;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.woodStorageLevel;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.woodStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.woodStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.woodStorageStartConstructionTime = startConstructionTime;
        }
    }
}
