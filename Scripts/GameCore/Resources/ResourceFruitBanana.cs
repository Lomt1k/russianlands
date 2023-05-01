using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    internal class ResourceFruitBanana : IResource
    {
        public ResourceId resourceId => ResourceId.FruitBanana;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceFruitBanana;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceFruitBanana = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceFruitBanana += value;
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
}
