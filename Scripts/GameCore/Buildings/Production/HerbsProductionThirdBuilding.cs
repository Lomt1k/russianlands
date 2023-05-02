using System;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Buildings.Production;

public class HerbsProductionThirdBuilding : ProductionBuildingBase
{
    public override ResourceId resourceId => ResourceId.Herbs;
    public override BuildingId buildingId => BuildingId.HerbsProductionThird;
    public override Avatar firstWorkerIcon => Avatar.FemaleI;
    public override Avatar secondWorkerIcon => Avatar.FemaleD;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.herbsProdThirdLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.herbsProdThirdLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.herbsProdThirdStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.herbsProdThirdStartConstructionTime = startConstructionTime;
    }

    public override DateTime GetStartFarmTime(ProfileBuildingsData data)
    {
        return data.herbsProdThirdStartFarmTime;
    }

    public override void SetStartFarmTime(ProfileBuildingsData data, DateTime startFarmTime)
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
