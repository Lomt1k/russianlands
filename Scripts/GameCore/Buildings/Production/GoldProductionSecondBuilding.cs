using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class GoldProductionSecondBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Gold;
        public override BuildingType buildingType => BuildingType.GoldProductionSecond;
        public override Avatar firstWorkerIcon => Avatar.Male;
        public override Avatar secondWorkerIcon => Avatar.MaleD;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.goldProdSecondLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.goldProdSecondLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.goldProdSecondStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.goldProdSecondStartConstructionTime = startConstructionTime;
        }

        public override long GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.goldProdSecondStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime)
        {
            data.goldProdSecondStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.goldProdSecondWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.goldProdSecondWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.goldProdSecondWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.goldProdSecondWorkerSecond = level;
        }

    }
}
