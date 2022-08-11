using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceFood : IResource
    {
        public ResourceType resourceType => ResourceType.Food;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceFood;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceFood = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceFood += value;
        }

        public bool IsUnlocked(ProfileData profileData)
        {
            return true;
        }

        public int GetResourceLimit(ProfileData profileData)
        {
            //TODO: Add limit logic
            return int.MaxValue;
        }

    }
}
