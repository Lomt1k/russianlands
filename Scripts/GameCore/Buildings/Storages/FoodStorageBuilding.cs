using System;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.GameCore.Buildings.Storages;

public class FoodStorageBuilding : StorageBuildingBase
{
    public override ResourceId resourceId => ResourceId.Food;
    public override BuildingId buildingId => BuildingId.FoodStorage;
    public override int resourceLimitForZeroLevel => 2_000;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.foodStorageLevel;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.foodStorageStartConstructionTime;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.foodStorageLevel = level;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.foodStorageStartConstructionTime = startConstructionTime;
    }
}
