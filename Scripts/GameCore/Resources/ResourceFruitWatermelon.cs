using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    internal class ResourceFruitWatermelon : IResource
    {
        public ResourceId resourceId => ResourceId.FruitWatermelon;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceFruitWatermelon;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceFruitWatermelon = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceFruitWatermelon += value;
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
