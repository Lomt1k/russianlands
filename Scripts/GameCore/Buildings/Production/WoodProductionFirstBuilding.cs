using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class WoodProductionFirstBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Wood;
        public override BuildingType buildingType => BuildingType.WoodProductionFirst;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.woodProdFirstLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.woodProdFirstLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.woodProdFirstStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.woodProdFirstStartConstructionTime = startConstructionTime;
        }

        public override long GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.woodProdFirstStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime)
        {
            data.woodProdFirstStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.woodProdFirstWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.woodProdFirstWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.woodProdFirstWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.woodProdFirstWorkerSecond = level;
        }

    }
}
