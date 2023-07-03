using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitApple : IResource
{
    public ResourceId resourceId => ResourceId.FruitApple;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitApple;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitApple = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitApple += value;
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
