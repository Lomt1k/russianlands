using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages
{
    public class ItemsStorageBuilding : StorageBuildingBase
    {
        public override ResourceId resourceId => ResourceId.InventoryItems;
        public override BuildingId buildingId => BuildingId.ItemsStorage;
        public override int resourceLimitForZeroLevel => 50;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.itemsStorageLevel;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.itemsStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.itemsStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.itemsStorageStartConstructionTime = startConstructionTime;
        }
    }
}
