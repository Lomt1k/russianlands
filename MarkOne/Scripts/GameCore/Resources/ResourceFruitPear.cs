using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitPear : IResource
{
    public ResourceId resourceId => ResourceId.FruitPear;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitPear;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitPear = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitPear += value;
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
