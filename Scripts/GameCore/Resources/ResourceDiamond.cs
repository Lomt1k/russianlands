using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceDiamond : IResource
    {
        public ResourceType resourceType => ResourceType.Diamond;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceDiamond;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceDiamond = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceDiamond += value;
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
