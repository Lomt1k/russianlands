using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitBlueberry : IResource
{
    public ResourceId resourceId => ResourceId.FruitBlueberry;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitBlueberry;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitBlueberry = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitBlueberry += value;
    }

    public bool IsUnlocked(GameSession session)
    {
        // ignored
        return true;
    }

    public int GetResourceLimit(GameSession session)
    {
        return int.MaxValue;
    }

}
