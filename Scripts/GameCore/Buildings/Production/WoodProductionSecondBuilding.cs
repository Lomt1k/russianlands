using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Units;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class WoodProductionSecondBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Wood;
        public override BuildingType buildingType => BuildingType.WoodProductionSecond;
        public override Avatar firstWorkerIcon => Avatar.MaleI;
        public override Avatar secondWorkerIcon => Avatar.MaleH;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.woodProdSecondLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.woodProdSecondLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.woodProdSecondStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.woodProdSecondStartConstructionTime = startConstructionTime;
        }

        public override DateTime GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.woodProdSecondStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
        {
            data.woodProdSecondStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.woodProdSecondWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.woodProdSecondWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.woodProdSecondWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.woodProdSecondWorkerSecond = level;
        }

    }
}
