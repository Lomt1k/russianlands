using System;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production;

public class FoodProductionThirdBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Food;
    public override BuildingId buildingId => BuildingId.FoodProductionThird;
    public override Avatar firstWorkerIcon => Avatar.FemaleD;
    public override Avatar secondWorkerIcon => Avatar.MaleD;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.foodProdThirdLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.foodProdThirdLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.foodProdThirdStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.foodProdThirdStartConstructionTime = startConstructionTime;
    }

    public override DateTime GetStartFarmTime(ProfileBuildingsData data)
    {
        return data.foodProdThirdStartFarmTime;
    }

    public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
    {
        data.foodProdThirdStartFarmTime = startFarmTime;
    }

    public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
    {
        return data.foodProdThirdWorkerFirst;
    }

    public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
    {
        return data.foodProdThirdWorkerSecond;
    }

    public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
    {
        data.foodProdThirdWorkerFirst = level;
    }

    public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
    {
        data.foodProdThirdWorkerSecond = level;
    }

}
