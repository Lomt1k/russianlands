using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public interface IResource
{
    public ResourceId resourceId { get; }

    int GetValue(ProfileData profileData);
    void SetValue(ProfileData profileData, int value);
    void AddValue(ProfileData profileData, int value);
    bool IsUnlocked(GameSession session);
    int GetResourceLimit(GameSession session);
}
