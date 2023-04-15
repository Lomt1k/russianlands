﻿using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Units;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production
{
    public class HerbsProductionSecondBuilding : ProductionBuildingBase
    {
        public override ResourceType resourceType => ResourceType.Herbs;
        public override BuildingType buildingType => BuildingType.HerbsProductionSecond;
        public override Avatar firstWorkerIcon => Avatar.FemaleE;
        public override Avatar secondWorkerIcon => Avatar.FemaleC;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.herbsProdSecondLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdSecondLevel = level;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.herbsProdSecondStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.herbsProdSecondStartConstructionTime = startConstructionTime;
        }

        public override DateTime GetStartFarmTime(ProfileBuildingsData data)
        {
            return data.herbsProdSecondStartFarmTime;
        }

        public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
        {
            data.herbsProdSecondStartFarmTime = startFarmTime;
        }

        public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
        {
            return data.herbsProdSecondWorkerFirst;
        }

        public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
        {
            return data.herbsProdSecondWorkerSecond;
        }

        public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdSecondWorkerFirst = level;
        }

        public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
        {
            data.herbsProdSecondWorkerSecond = level;
        }

    }
}
