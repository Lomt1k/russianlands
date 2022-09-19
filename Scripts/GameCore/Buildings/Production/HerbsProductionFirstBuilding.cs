using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class HerbsProductionFirstBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Herbs;
        public override BuildingType buildingType => BuildingType.HerbsProductionFirst;
        public override CharIcon firstWorkerIcon => CharIcon.Female;
        public override CharIcon secondWorkerIcon => CharIcon.FemaleB;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.herbsProdFirstLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdFirstLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.herbsProdFirstStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.herbsProdFirstStartConstructionTime = startConstructionTime;
        }

        public override long GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.herbsProdFirstStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime)
        {
            data.herbsProdFirstStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.herbsProdFirstWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.herbsProdFirstWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdFirstWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdFirstWorkerSecond = level;
        }

    }
}
