using System;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Buildings.Production;

public class GoldProductionFirstBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Gold;
    public override BuildingId buildingId => BuildingId.GoldProductionFirst;
    public override AvatarId firstWorkerIcon => AvatarId.Male_01;
    public override AvatarId secondWorkerIcon => AvatarId.Male_08;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.goldProdFirstLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.goldProdFirstLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.goldProdFirstStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.goldProdFirstStartConstructionTime = startConstructionTime;
    }

    public override DateTime GetStartFarmTime(ProfileBuildingsData data)
    {
        return data.goldProdFirstStartFarmTime;
    }

    public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
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

    public override int GetStorageResourceAmount(ProfileBuildingsData data)
    {
        return data.goldProdFirstStorageAmount;
    }

    public override void SetStorageResourceAmount(ProfileBuildingsData data, int resourceAmount)
    {
        data.goldProdFirstStorageAmount = resourceAmount;
    }

}
