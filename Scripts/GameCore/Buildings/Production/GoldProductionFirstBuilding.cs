using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class GoldProductionFirstBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Gold;
        public override BuildingType buildingType => BuildingType.GoldProductionFirst;
        public override Avatar firstWorkerIcon => Avatar.MaleB;
        public override Avatar secondWorkerIcon => Avatar.MaleI;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.goldProdFirstLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.goldProdFirstLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.goldProdFirstStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.goldProdFirstStartConstructionTime = startConstructionTime;
        }

        public override long GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.goldProdFirstStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime)
        {
            data.goldProdFirstStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.goldProdFirstWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.goldProdFirstWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.goldProdFirstWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.goldProdFirstWorkerSecond = level;
        }
        
    }
}
