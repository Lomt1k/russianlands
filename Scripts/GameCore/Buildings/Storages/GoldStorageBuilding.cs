using System;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Buildings.Storages;

public class GoldStorageBuilding : StorageBuildingBase
{
    public override ResourceId resourceId => ResourceId.Gold;
    public override BuildingId buildingId => BuildingId.GoldStorage;
    public override int resourceLimitForZeroLevel => 5_000;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.goldStorageLevel;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.goldStorageStartConstructionTime;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.goldStorageLevel = level;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.goldStorageStartConstructionTime = startConstructionTime;
    }
}
