﻿using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class HerbsProductionThirdBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Herbs;
        public override BuildingType buildingType => BuildingType.HerbsProductionThird;
        public override CharIcon firstWorkerIcon => CharIcon.FemaleI;
        public override CharIcon secondWorkerIcon => CharIcon.FemaleD;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.herbsProdThirdLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdThirdLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.herbsProdThirdStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.herbsProdThirdStartConstructionTime = startConstructionTime;
        }

        public override long GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.herbsProdThirdStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, long startFarmTime)
        {
            data.herbsProdThirdStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.herbsProdThirdWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.herbsProdThirdWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdThirdWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdThirdWorkerSecond = level;
        }

    }
}
