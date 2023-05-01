﻿using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using System;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages
{
    public class WoodStorageBuilding : StorageBuildingBase
    {
        public override ResourceId resourceId => ResourceId.Wood;
        public override BuildingId buildingId => BuildingId.WoodStorage;
        public override int resourceLimitForZeroLevel => 50_000;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.woodStorageLevel;
        }

        protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.woodStorageStartConstructionTime;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.woodStorageLevel = level;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
        {
            data.woodStorageStartConstructionTime = startConstructionTime;
        }
    }
}
