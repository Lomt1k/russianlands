using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitKiwi : IResource
{
    public ResourceId resourceId => ResourceId.FruitKiwi;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitKiwi;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitKiwi = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitKiwi += value;
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
