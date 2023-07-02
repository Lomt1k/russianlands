using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;
internal class ResourceArenaChip : IResource
{
    public ResourceId resourceId => ResourceId.ArenaChip;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceArenaChip;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceArenaChip = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceArenaChip += value;
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
