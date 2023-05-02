using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources;

public class ResourceDiamond : IResource
{
    public ResourceId resourceId => ResourceId.Diamond;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceDiamond;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceDiamond = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceDiamond += value;
    }

    public bool IsUnlocked(GameSession session)
    {
        return true;
    }

    public int GetResourceLimit(GameSession session)
    {
        return int.MaxValue;
    }

}
