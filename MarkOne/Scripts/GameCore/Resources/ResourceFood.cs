﻿using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public class ResourceFood : IResource
{
    public ResourceId resourceId => ResourceId.Food;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFood;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFood = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFood += value;
    }

    public bool IsUnlocked(GameSession session)
    {
        return true;
    }

    public int GetResourceLimit(GameSession session)
    {
        var buildingData = session.profile.buildingsData;
        var storage = (StorageBuildingBase)BuildingId.FoodStorage.GetBuilding();
        return storage.GetCurrentLevelResourceLimit(buildingData);
    }

}
