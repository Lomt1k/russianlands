using System;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Buildings.Production;

public class FoodProductionSecondBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Food;
    public override BuildingId buildingId => BuildingId.FoodProductionSecond;
    public override AvatarId firstWorkerIcon => AvatarId.Male_05;
    public override AvatarId secondWorkerIcon => AvatarId.Male_07;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.foodProdSecondLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.foodProdSecondLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.foodProdSecondStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.foodProdSecondStartConstructionTime = startConstructionTime;
    }

    public override DateTime GetStartFarmTime(ProfileBuildingsData data)
    {
        return data.foodProdSecondStartFarmTime;
    }

    public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
    {
        data.foodProdSecondStartFarmTime = startFarmTime;
    }

    public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
    {
        return data.foodProdSecondWorkerFirst;
    }

    public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
    {
        return data.foodProdSecondWorkerSecond;
    }

    public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
    {
        data.foodProdSecondWorkerFirst = level;
    }

    public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
    {
        data.foodProdSecondWorkerSecond = level;
    }

    public override int GetStorageResourceAmount(ProfileBuildingsData data)
    {
        return data.foodProdSecondStorageAmount;
    }

    public override void SetStorageResourceAmount(ProfileBuildingsData data, int resourceAmount)
    {
        data.foodProdSecondStorageAmount = resourceAmount;
    }

}
