﻿using System;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Buildings.Production;

public class WoodProductionSecondBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Wood;
    public override BuildingId buildingId => BuildingId.WoodProductionSecond;
    public override AvatarId firstWorkerIcon => AvatarId.Male_08;
    public override AvatarId secondWorkerIcon => AvatarId.Male_07;

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

    public override int GetStorageResourceAmount(ProfileBuildingsData data)
    {
        return data.woodProdSecondStorageAmount;
    }

    public override void SetStorageResourceAmount(ProfileBuildingsData data, int resourceAmount)
    {
        data.woodProdSecondStorageAmount = resourceAmount;
    }

}
