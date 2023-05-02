using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitStrawberry : IResource
{
    public ResourceId resourceId => ResourceId.FruitStrawberry;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitStrawberry;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitStrawberry = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitStrawberry += value;
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
