using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitCoconut : IResource
{
    public ResourceId resourceId => ResourceId.FruitCoconut;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitCoconut;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitCoconut = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitCoconut += value;
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
