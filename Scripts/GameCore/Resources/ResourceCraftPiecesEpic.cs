using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources;

public class ResourceCraftPiecesEpic : IResource
{
    public ResourceId resourceId => ResourceId.CraftPiecesEpic;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceCraftPiecesEpic;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesEpic = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesEpic += value;
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
