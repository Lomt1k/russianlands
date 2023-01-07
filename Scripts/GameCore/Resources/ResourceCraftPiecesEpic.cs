using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceCraftPiecesEpic : IResource
    {
        public ResourceType resourceType => ResourceType.CraftPiecesEpic;

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
            return true;
        }

        public int GetResourceLimit(GameSession session)
        {
            return int.MaxValue;
        }

    }
}
