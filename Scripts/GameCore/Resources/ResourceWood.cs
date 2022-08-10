using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceWood : IResource
    {
        public ResourceType resourceType => ResourceType.Wood;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceWood;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceWood = value;
        }
    }
}
