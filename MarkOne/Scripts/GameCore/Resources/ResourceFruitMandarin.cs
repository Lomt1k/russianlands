using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitMandarin : IResource
{
    public ResourceId resourceId => ResourceId.FruitMandarin;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitMandarin;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitMandarin = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitMandarin += value;
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
