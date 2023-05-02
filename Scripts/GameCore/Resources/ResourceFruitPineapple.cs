using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitPineapple : IResource
{
    public ResourceId resourceId => ResourceId.FruitPineapple;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitPineapple;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitPineapple = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitPineapple += value;
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
