﻿using System;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;

namespace MarkOne.Scripts.GameCore.Buildings.Storages;

public class HerbsStorageBuilding : StorageBuildingBase
{
    public override ResourceId resourceId => ResourceId.Herbs;
    public override BuildingId buildingId => BuildingId.HerbsStorage;
    public override int resourceLimitForZeroLevel => 10_000;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.herbsStorageLevel;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.herbsStorageStartConstructionTime;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.herbsStorageLevel = level;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.herbsStorageStartConstructionTime = startConstructionTime;
    }
}
