using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceHerbs : IResource
    {
        public ResourceType resourceType => ResourceType.Herbs;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceHerbs;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceHerbs = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceHerbs += value;
        }

        public bool IsUnlocked(ProfileData profileData)
        {
            //TODO
            return false;
        }

        public int GetResourceLimit(ProfileData profileData)
        {
            //TODO: Add limit logic
            return int.MaxValue;
        }

    }
}
