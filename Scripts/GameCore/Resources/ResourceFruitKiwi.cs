using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    internal class ResourceFruitKiwi : IResource
    {
        public ResourceType resourceType => ResourceType.FruitKiwi;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceFruitKiwi;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceFruitKiwi = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceFruitKiwi += value;
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
