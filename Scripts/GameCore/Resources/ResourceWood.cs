using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public class ResourceWood : IResource
{
    public ResourceId resourceId => ResourceId.Wood;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceWood;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceWood = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceWood += value;
    }

    public bool IsUnlocked(GameSession session)
    {
        var buildingsData = session.profile.buildingsData;
        var building = BuildingId.TownHall.GetBuilding();
        return building.GetCurrentLevel(buildingsData) >= 6;
    }

    public int GetResourceLimit(GameSession session)
    {
        var buildingData = session.profile.buildingsData;
        var storage = (StorageBuildingBase)BuildingId.WoodStorage.GetBuilding();
        return storage.GetCurrentLevelResourceLimit(buildingData);
    }

}
