using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitGrape : IResource
{
    public ResourceId resourceId => ResourceId.FruitGrape;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitGrape;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitGrape = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitGrape += value;
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
