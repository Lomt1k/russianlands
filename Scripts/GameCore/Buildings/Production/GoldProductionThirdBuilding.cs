using System;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Buildings.Production;

public class GoldProductionThirdBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Gold;
    public override BuildingId buildingId => BuildingId.GoldProductionThird;
    public override Avatar firstWorkerIcon => Avatar.MaleH;
    public override Avatar secondWorkerIcon => Avatar.MaleE;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.goldProdThirdLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.goldProdThirdLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.goldProdThirdStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.goldProdThirdStartConstructionTime = startConstructionTime;
    }

    public override DateTime GetStartFarmTime(ProfileBuildingsData data)
    {
        return data.goldProdThirdStartFarmTime;
    }

    public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
    {
        data.goldProdThirdStartFarmTime = startFarmTime;
    }

    public override byte GetFirstWorkerLevel(ProfileBuildingsData data)
    {
        return data.goldProdThirdWorkerFirst;
    }

    public override byte GetSecondWorkerLevel(ProfileBuildingsData data)
    {
        return data.goldProdThirdWorkerSecond;
    }

    public override void SetFirstWorkerLevel(ProfileBuildingsData data, byte level)
    {
        data.goldProdThirdWorkerFirst = level;
    }

    public override void SetSecondWorkerLevel(ProfileBuildingsData data, byte level)
    {
        data.goldProdThirdWorkerSecond = level;
    }

}
