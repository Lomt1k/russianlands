using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages
{
    public class ItemsStorageBuilding : StorageBuildingBase
    {
        public override ResourceType resourceType => ResourceType.InventoryItems;
        public override BuildingType buildingType => BuildingType.ItemsStorage;
        public override int resourceLimitForZeroLevel => 50;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.itemsStorageLevel;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.itemsStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.itemsStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.itemsStorageStartConstructionTime = startConstructionTime;
        }
    }
}
