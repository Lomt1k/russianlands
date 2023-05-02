using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public class ResourceCraftPiecesLegendary : IResource
{
    public ResourceId resourceId => ResourceId.CraftPiecesLegendary;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceCraftPiecesLegendary;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesLegendary = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesLegendary += value;
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
