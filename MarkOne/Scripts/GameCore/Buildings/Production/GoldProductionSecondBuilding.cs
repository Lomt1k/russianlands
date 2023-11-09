using System;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Buildings.Production;

public class GoldProductionSecondBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Gold;
    public override BuildingId buildingId => BuildingId.GoldProductionSecond;
    public override AvatarId firstWorkerIcon => AvatarId.Male_00;
    public override AvatarId secondWorkerIcon => AvatarId.Male_03;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.goldProdSecondLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.goldProdSecondLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.goldProdSecondStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.goldProdSecondStartConstructionTime = startConstructionTime;
    }

    public override DateTime GetStartFarmTime(ProfileBuildingsData data)
    {
        return data.goldProdSecondStartFarmTime;
    }

    public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
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

    public override int GetStorageResourceAmount(ProfileBuildingsData data)
    {
        return data.goldProdSecondStorageAmount;
    }

    public override void SetStorageResourceAmount(ProfileBuildingsData data, int resourceAmount)
    {
        data.goldProdSecondStorageAmount = resourceAmount;
    }

}
