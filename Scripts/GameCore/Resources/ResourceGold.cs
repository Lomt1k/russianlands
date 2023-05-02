using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public class ResourceGold : IResource
{
    public ResourceId resourceId => ResourceId.Gold;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceGold;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceGold = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceGold += value;
    }

    public bool IsUnlocked(GameSession session)
    {
        return true;
    }

    public int GetResourceLimit(GameSession session)
    {
        var buildingData = session.profile.buildingsData;
        var storage = (StorageBuildingBase)BuildingId.GoldStorage.GetBuilding();
        return storage.GetCurrentLevelResourceLimit(buildingData);
    }

}
