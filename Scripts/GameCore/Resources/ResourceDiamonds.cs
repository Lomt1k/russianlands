using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceDiamonds : IResource
    {
        public ResourceType resourceType => ResourceType.Diamonds;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceDiamonds;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceDiamonds = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceDiamonds += value;
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
