using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public class ResourceCraftPiecesRare : IResource
{
    public ResourceId resourceId => ResourceId.CraftPiecesRare;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceCraftPiecesRare;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesRare = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesRare += value;
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
